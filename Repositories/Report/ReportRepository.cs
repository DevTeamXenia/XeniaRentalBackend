using Microsoft.EntityFrameworkCore;
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


        public async Task<List<TenantOccupancyReportDto>> GetTenantOccupancyReportAsync( int companyId, int? propertyId, int? unitId, bool isBedSpace, DateTime? month)
        {
     
            var voucherQuery = _context.Vouchers
                .AsNoTracking()
                .Where(v =>
                    v.CompanyID == companyId &&
                    v.isActive == true &&
                    v.Cancelled == false &&
                    v.VoucherType == "RECEIPT"
                );

            DateTime rentCalcEndDate = month.HasValue
                ? new DateTime(month.Value.Year, month.Value.Month, 1)
                      .AddMonths(1)
                      .AddDays(-1)
                : DateTime.Today;

            voucherQuery = voucherQuery.Where(v => v.VoucherDate <= rentCalcEndDate);

            var tenantPaidAmounts = await voucherQuery
                .GroupBy(v => v.CrID)
                .Select(g => new
                {
                    TenantId = g.Key,
                    Paid = g.Sum(x => x.Amount)
                })
                .ToDictionaryAsync(x => x.TenantId, x => x.Paid);

            var baseQuery =
                from ta in _context.TenantAssignemnts.AsNoTracking()
                join p in _context.Properties on ta.propID equals p.PropID
                join u in _context.Units on ta.unitID equals u.UnitId
                join t in _context.Tenants on ta.tenantID equals t.tenantID
                join b in _context.BedSpaces on ta.bedSpaceID equals b.bedID into bs
                from bed in bs.DefaultIfEmpty()
                where ta.companyID == companyId
                      && ta.isClosure == false
                select new
                {
                    p.PropID,
                    PropertyName = p.propertyName,

                    u.UnitId,
                    UnitName = u.UnitName,

                    BedSpaceId = ta.bedSpaceID > 0 ? ta.bedSpaceID : null,
                    BedSpaceName = ta.bedSpaceID > 0 ? bed.bedSpaceName : null,

                    t.tenantID,
                    TenantName = t.tenantName,
                    Phone = t.phoneNumber,

                    Rent = ta.rentAmt,
                    Deposit = ta.securityAmt,

                    JoinDate = ta.agreementStartDate,
                    EndDate = ta.agreementEndDate
                };

            if (propertyId.HasValue)
                baseQuery = baseQuery.Where(x => x.PropID == propertyId.Value);

            if (unitId.HasValue)
                baseQuery = baseQuery.Where(x => x.UnitId == unitId.Value);

            if (isBedSpace)
                baseQuery = baseQuery.Where(x => x.BedSpaceId != null);

            if (month.HasValue)
            {
                var start = new DateTime(month.Value.Year, month.Value.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);

                baseQuery = baseQuery.Where(x =>
                    x.JoinDate <= end &&
                    x.EndDate >= start);
            }

            var data = await baseQuery.ToListAsync();   
            return data
                .GroupBy(p => new { p.PropID, p.PropertyName })
                .Select(prop => new TenantOccupancyReportDto
                {
                    PropertyId = prop.Key.PropID,
                    PropertyName = prop.Key.PropertyName,

                    Units = prop
                        .GroupBy(u => new { u.UnitId, u.UnitName })
                        .Select(unit =>
                        {
                            var unitDto = new UnitReportDto
                            {
                                UnitId = unit.Key.UnitId,
                                UnitName = unit.Key.UnitName
                            };

         
                            var fullUnitTenants = unit
                                .Where(x => x.BedSpaceId == null)
                                .Select(t => CreateTenantRow(
                                    t,
                                    tenantPaidAmounts,
                                    rentCalcEndDate))
                                .ToList();

                            if (fullUnitTenants.Any())
                                unitDto.Tenants = fullUnitTenants;

             
                            var bedSpaces = unit
                                .Where(x => x.BedSpaceId != null)
                                .GroupBy(b => new { b.BedSpaceId, b.BedSpaceName })
                                .Select(bs => new BedSpaceReportDto
                                {
                                    BedSpaceId = bs.Key.BedSpaceId,
                                    BedSpaceName = bs.Key.BedSpaceName,
                                    Tenants = bs
                                        .Select(t => CreateTenantRow(
                                            t,
                                            tenantPaidAmounts,
                                            rentCalcEndDate))
                                        .ToList()
                                })
                                .Where(bs => bs.Tenants.Any())
                                .ToList();

                            if (bedSpaces.Any())
                                unitDto.BedSpaces = bedSpaces;

                            return unitDto;
                        })
                        .ToList()
                })
                .ToList();
        }


        private int GetMonthsBetween(DateTime start, DateTime end)
        {
            if (end < start)
                return 0;

            return ((end.Year - start.Year) * 12) + end.Month - start.Month + 1;
        }


        private TenantRowDto CreateTenantRow( dynamic t, Dictionary<int, decimal> tenantPaidAmounts, DateTime rentCalcEndDate)
        {
            var months = GetMonthsBetween(t.JoinDate, rentCalcEndDate);
            var expectedRent = months * t.Rent;

            var paid = tenantPaidAmounts.ContainsKey(t.tenantID)
                ? tenantPaidAmounts[t.tenantID]
                : 0;

            return new TenantRowDto
            {
                TenantId = t.tenantID,
                TenantName = t.TenantName,
                Phone = t.Phone,
                Deposit = t.Deposit,
                Rent = t.Rent,
                Balance = expectedRent - paid,
                JoinDate = t.JoinDate,
                Status = t.Deposit > 0 ? "New" : "Set Off"
            };
        }



    }
}
