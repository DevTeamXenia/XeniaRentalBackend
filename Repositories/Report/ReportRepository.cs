using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Dtos.Report;
using XeniaRentalBackend.Dtos.Reports;
using XeniaRentalBackend.Models;
namespace XeniaRentalBackend.Repositories.Report
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<TenantOccupancyReportDto>> GetTenantOccupancyReportAsync(int companyId, int? propertyId, int? unitId, int? bedSpaceId, bool isBedSpace, string? search)
        {
            var today = DateTime.Today;

            var tenantPaidAmounts = await _context.Vouchers
                .AsNoTracking()
                .Where(v =>
                    v.CompanyID == companyId &&
                    v.isActive &&
                    !v.Cancelled &&
                    v.VoucherType == "RECEIPT")
                .GroupBy(v => v.CrID)
                .Select(g => new { TenantId = g.Key, Paid = g.Sum(x => x.Amount) })
                .ToDictionaryAsync(x => x.TenantId, x => x.Paid);

            List<XRS_Bedspace> bedSpaces;

            if (isBedSpace)
            {
                bedSpaces = await _context.BedSpaces
                    .AsNoTracking()
                    .Where(b => b.companyID == companyId && b.isActive)
                    .ToListAsync();
            }
            else
            {
                bedSpaces = new List<XRS_Bedspace>();
            }

 
            var unitIdsWithBedSpaces = await _context.BedSpaces
                .AsNoTracking()
                .Where(b => b.companyID == companyId && b.isActive)
                .Select(b => b.unitID)
                .Distinct()
                .ToListAsync();

      
            var tenantQuery =
                from ta in _context.TenantAssignemnts.AsNoTracking()
                join t in _context.Tenants on ta.tenantID equals t.tenantID
                where ta.companyID == companyId && !ta.isClosure
                select new
                {
                    ta.propID,
                    ta.unitID,
                    BedSpaceId = ta.bedSpaceID > 0 ? ta.bedSpaceID : (int?)null,
                    t.tenantID,
                    TenantName = t.tenantName,
                    Phone = t.phoneNumber,
                    Rent = ta.rentAmt,
                    Deposit = ta.securityAmt,
                    JoinDate = ta.agreementStartDate,
                    EndDate = ta.agreementEndDate,
                    DueDate = "",
                    Note = ta.notes
                };

            if (bedSpaceId.HasValue)
                tenantQuery = tenantQuery.Where(x => x.BedSpaceId == bedSpaceId);

            if (!string.IsNullOrWhiteSpace(search))
                tenantQuery = tenantQuery.Where(x =>
                    x.TenantName.Contains(search) ||
                    x.Phone.Contains(search));

            var tenants = await tenantQuery.ToListAsync();
      
            var properties = await (
                from p in _context.Properties
                join u in _context.Units on p.PropID equals u.PropID
                where p.CompanyId == companyId
                      && (
                            isBedSpace
                                ? unitIdsWithBedSpaces.Contains(u.UnitId)
                                : !unitIdsWithBedSpaces.Contains(u.UnitId)
                         )
                select new
                {
                    p.PropID,
                    PropertyName = p.propertyName,
                    u.UnitId,
                    UnitName = u.UnitName
                }).ToListAsync();

            if (propertyId.HasValue)
                properties = properties.Where(x => x.PropID == propertyId).ToList();

            if (unitId.HasValue)
                properties = properties.Where(x => x.UnitId == unitId).ToList();

  
            return properties
                .GroupBy(p => new { p.PropID, p.PropertyName })
                .Select(prop => new TenantOccupancyReportDto
                {
                    PropertyId = prop.Key.PropID,
                    PropertyName = prop.Key.PropertyName,

                    Units = prop.GroupBy(u => new { u.UnitId, u.UnitName })
                        .Select(unit =>
                        {
                            var unitTenants = tenants
                                .Where(t => t.unitID == unit.Key.UnitId)
                                .ToList();

                            var unitDto = new UnitReportDto
                            {
                                UnitId = unit.Key.UnitId,
                                UnitName = unit.Key.UnitName,
                                TotalBedSpaces = isBedSpace
                                    ? bedSpaces
                                        .Where(b => b.unitID == unit.Key.UnitId && b.propID == prop.Key.PropID)
                                        .Sum(b => b.bedSpaceCount)
                                    : 0
                            };

                            if (isBedSpace)
                            {
                                var unitBedSpaces = bedSpaces
                                    .Where(b => b.unitID == unit.Key.UnitId && b.propID == prop.Key.PropID)
                                    .ToList();

                                unitDto.BedSpaces = unitBedSpaces.Select(bs =>
                                {
                                    var tenantsInBedSpace = unitTenants
                                        .Where(t => t.BedSpaceId == bs.bedID)
                                        .ToList();

                                    return new BedSpaceReportDto
                                    {
                                        BedSpaceId = bs.bedID,
                                        BedSpaceName = bs.bedSpaceName,
                                        TotalBedSpaces = bs.bedSpaceCount,
                                        OccupiedBedSpaces = tenantsInBedSpace.Count,
                                        Tenants = tenantsInBedSpace
                                            .Select(t => CreateTenantRow(t, tenantPaidAmounts))
                                            .ToList()
                                    };
                                }).ToList();
                            }                    
                            else
                            {
                                unitDto.Tenants = unitTenants
                                    .Select(t => CreateTenantRow(t, tenantPaidAmounts))
                                    .ToList();

                                unitDto.BedSpaces = null;
                            }

                            return unitDto;
                        }).ToList()
                }).ToList();
        }

        public async Task<BalanceSheetResponseDto> GetIncomeExpenseAsync( int companyId, DateTime? startDate,  DateTime? endDate,  int? propertyId)
        {
            var rawData = await (
                from a in _context.Accounts
                join v in _context.Vouchers on a.VoucherId equals v.VoucherID
                join g in _context.AccountGroups on a.GroupId equals g.groupID
                join ldr in _context.Ledgers on a.ledgerDr equals ldr.ledgerID into drJoin
                from dr in drJoin.DefaultIfEmpty()
                join lcr in _context.Ledgers on a.ledgerCr equals lcr.ledgerID into crJoin
                from cr in crJoin.DefaultIfEmpty()
                where a.companyID == companyId
                      && a.isActive
                      && (!startDate.HasValue || v.VoucherDate >= startDate.Value)
                      && (!endDate.HasValue || v.VoucherDate <= endDate.Value)
                      && (!propertyId.HasValue || v.PropID == propertyId.Value)
                select new
                {
                    g.groupName,
                    g.groupCode,
                    v.VoucherType,
                    LedgerName = a.amountDr > 0 ? dr.ledgerName : cr.ledgerName,
                    AmountDr = a.amountDr,
                    AmountCr = a.amountCr
                }
            ).AsNoTracking().ToListAsync();

            bool IsIncome(dynamic x)
            {
                if (x.groupCode == "INDIRECT EXPENSES")
                    return false;

                if (x.groupCode == "INDIRECT INCOME" && x.VoucherType == "Rent Pay")
                    return true;

                return x.AmountCr > 0;
            }

            var incomeData = rawData.Where(IsIncome).ToList();
            var expenseData = rawData.Where(x => !IsIncome(x)).ToList();

            var incomeGroups = incomeData
                .GroupBy(x => x.groupName)
                .Select(g => new GroupSummaryDto
                {
                    GroupName = g.Key,
                    GroupTotal = g.Sum(x => x.AmountCr),
                    Ledgers = g.GroupBy(x => x.LedgerName)
                        .Select(l => new LedgerTotalDto
                        {
                            LedgerName = l.Key!,
                            Amount = l.Sum(x => x.AmountCr)
                        })
                        .ToList()
                })
                .ToList();

            var expenseGroups = expenseData
                .GroupBy(x => x.groupName)
                .Select(g => new GroupSummaryDto
                {
                    GroupName = g.Key,
                    GroupTotal = g.Sum(x => x.AmountDr),
                    Ledgers = g.GroupBy(x => x.LedgerName)
                        .Select(l => new LedgerTotalDto
                        {
                            LedgerName = l.Key!,
                            Amount = l.Sum(x => x.AmountDr)
                        })
                        .ToList()
                })
                .ToList();

            return new BalanceSheetResponseDto
            {
                IncomeGroups = incomeGroups,
                ExpenseGroups = expenseGroups,
                TotalIncome = incomeGroups.Sum(x => x.GroupTotal),
                TotalExpense = expenseGroups.Sum(x => x.GroupTotal)
            };
        }


        private int GetMonthsBetween(DateTime start, DateTime end)
        {
            if (end < start) return 0;
            int months = (end.Year - start.Year) * 12 + (end.Month - start.Month);
            if (end.Day >= start.Day) months++;
            return months;
        }

        private TenantRowDto CreateTenantRow(dynamic t,Dictionary<int, decimal> tenantPaidAmounts)
        {
            var today = DateTime.Today;

            DateTime joinDate = today;

            DateTime parsedJoinDate = today;

            if (t.JoinDate != null &&
                !string.IsNullOrWhiteSpace(t.JoinDate.ToString()) &&
                DateTime.TryParse(t.JoinDate.ToString(), out parsedJoinDate))
            {
                joinDate = parsedJoinDate;
            }

            DateTime? endDate = null;

            DateTime parsedEndDate = today;

            if (t.EndDate != null &&
                !string.IsNullOrWhiteSpace(t.EndDate.ToString()) &&
                DateTime.TryParse(t.EndDate.ToString(), out parsedEndDate))
            {
                endDate = parsedEndDate;
            }

            DateTime rentEndDate =
                endDate.HasValue && endDate.Value < today
                    ? endDate.Value
                    : today;

   
            decimal rent = 0;

            decimal parsedRent = 0;

            if (t.Rent != null &&
                decimal.TryParse(t.Rent.ToString(), out parsedRent))
            {
                rent = parsedRent;
            }


            int months = GetMonthsBetween(joinDate, rentEndDate);

            decimal expectedRent = months * rent;

            int tenantId = 0;

            if (t.tenantID != null)
            {
                int.TryParse(t.tenantID.ToString(), out tenantId);
            }

  
            decimal paid = tenantPaidAmounts.TryGetValue(
                tenantId,
                out decimal amount)
                    ? amount
                    : 0m;

            decimal balance = expectedRent - paid;


            DateTime? parsedDueDate = null;

            DateTime dueDate = today;

            if (t.DueDate != null &&
                !string.IsNullOrWhiteSpace(t.DueDate.ToString()) &&
                DateTime.TryParse(t.DueDate.ToString(), out dueDate))
            {
                parsedDueDate = dueDate;
            }

            decimal deposit = 0;

            if (t.Deposit != null)
            {
                decimal.TryParse(t.Deposit.ToString(), out deposit);
            }

            return new TenantRowDto
            {
                TenantId = tenantId,

                TenantName = t.TenantName?.ToString(),

                Phone = t.Phone?.ToString(),

                Deposit = deposit,

                Rent = rent,

                Balance = balance,

                JoinDate = joinDate,

                EndDate = endDate,

                RentDueDate = parsedDueDate,

                Note = t.Note?.ToString(),

                Status = balance > 0
                    ? "Pending"
                    : "Clear"
            };
        }


        public async Task<List<MaintenanceReportDto>> GetMaintenanceReport(int companyId, int? tenantId, string? status,DateTime? fromDate, DateTime? toDate, string? zone, string? search)
        {
            var now = DateTime.Now;

            var categories = await _context.MaintenanceCategories
                .Where(c => c.CompanyId == companyId && c.IsActive)
                .ToListAsync();

            var categoryDict = categories.ToDictionary(c => c.CategoryId, c => c.SLADays);

            bool IsOverdue(XRS_Maintenance m)
            {
                if (m.Status != "Pending" && m.Status != "InProgress")
                    return false;

                if (!categoryDict.TryGetValue(m.CategoryId, out int slaDays))
                    return false;

                if (slaDays <= 0)
                    return false;

                return m.CreatedAt.AddDays(slaDays) < now;
            }

   
            var query = from m in _context.ManageMaintenance
                        join p in _context.Properties on m.PropertyId equals p.PropID into pp
                        from property in pp.DefaultIfEmpty()
                        join u in _context.Units on m.UnitId equals u.UnitId into uu
                        from unit in uu.DefaultIfEmpty()
                        join t in _context.Tenants on m.TenantId equals t.tenantID into tt
                        from tenant in tt.DefaultIfEmpty()
                        join c in _context.MaintenanceCategories on m.CategoryId equals c.CategoryId into cc
                        from category in cc.DefaultIfEmpty()
                        join e in _context.Employee on m.AssignedEmployeeId equals e.EmployeeId into ee
                        from employee in ee.DefaultIfEmpty()
                        where m.CompanyId == companyId && m.IsActive
                        select new { m, property, unit, tenant, category, employee };

   
            if (tenantId.HasValue && tenantId > 0)
                query = query.Where(x => x.m.TenantId == tenantId);

            if (fromDate.HasValue)
                query = query.Where(x => x.m.CreatedAt.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.m.CreatedAt.Date <= toDate.Value.Date);

            if (!string.IsNullOrEmpty(zone) && zone != "All")
                query = query.Where(x => x.employee != null && x.employee.AreaZone.Contains(zone));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.m.ComplaintNo.Contains(search) ||
                    x.m.Complaint.Contains(search) ||
                    (x.tenant != null && x.tenant.tenantName.Contains(search)) ||
                    (x.employee != null && x.employee.Name.Contains(search)));
            }

            var data = await query.ToListAsync();

            var result = data.Select(x => new MaintenanceReportDto
            {
                MaintenanceId = x.m.MaintenanceId,
                ComplaintNo = x.m.ComplaintNo,
                CreatedAt = x.m.CreatedAt,
                PropertyUnit = (x.property != null ? x.property.propertyName : "") +
                               (x.unit != null ? " - " + x.unit.UnitName : ""),
                RegisteredBy = x.tenant != null ? x.tenant.tenantName : "Owner",
                CategoryName = x.category != null ? x.category.CategoryName : "",
                Complaint = x.m.Complaint,
                Status = IsOverdue(x.m) ? "Overdue" : x.m.Status,

                EngineerName = x.employee != null ? x.employee.Name : "Unassigned",
                Zone = x.employee != null ? x.employee.AreaZone : "",
                UpdatedAt = x.m.UpdatedAt
            }).ToList();
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                result = result.Where(x => x.Status == status).ToList();
            }

            return result
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

    }
}
