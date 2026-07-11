using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Payment;
using XeniaTenoraBackend.Dtos;

namespace XeniaRentalBackend.Repositories.Voucher
{
    public class VoucherRepository:IVoucherRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;

        public VoucherRepository(ApplicationDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<IEnumerable<object>> GetAllExpenseVouchersAsync(int companyId, DateTime? fromDate, DateTime? toDate, int? propertyId, string? voucherStatus, string? search)
        {
            var query =
                from v in _context.Vouchers
                where v.VoucherType == "Expense Voucher"

                join dr in _context.Ledgers
                    on (int?)v.DrID equals (int?)dr.ledgerID into drJoin
                from dr in drJoin.DefaultIfEmpty()

         
                join crLedger in _context.Ledgers
                    on (int?)v.CrID equals (int?)crLedger.ledgerID into crLedgerJoin
                from crLedger in crLedgerJoin.DefaultIfEmpty()

           

                join p in _context.Properties
                    on (int?)v.PropID equals (int?)p.PropID into propJoin
                from p in propJoin.DefaultIfEmpty()

                where v.CompanyID == companyId

                select new
                {
                    v.VoucherID,
                    v.VoucherNo,
                    v.VoucherDate,
                    v.VoucherType,
                    v.VoucherStatus,
                    v.Amount,
                    v.RefNo,
                    v.Remarks,
                    v.IssueingBank,
                    v.ChequeNo,
                    v.Cancelled,
                    v.CrAmount,
                    v.IsReconcil,
                    v.ChequeStatus,
                    v.ReconcilDate,
                    v.CreatedOn,
                    v.CreatedBy,
                    v.ModificationBy,
                    v.isActive,
                    DrID = v.DrID,
                    DrName = dr != null ? dr.ledgerName : null,
                    CrID = v.CrID,
                    CrName =crLedger.ledgerName ,         
                    PropertyId = p.PropID,
                    PropertyName = p.propertyName
                };


            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(v =>
                    v.VoucherNo.Contains(search) ||
                    v.CrName.Contains(search) ||
                    v.PropertyName.Contains(search));
            }

            if (fromDate.HasValue)
                query = query.Where(v => v.VoucherDate >= fromDate.Value);

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);
                query = query.Where(v => v.VoucherDate < endDate);
            }

            if (propertyId.HasValue)
                query = query.Where(v => v.PropertyId == propertyId.Value);


            if (!string.IsNullOrWhiteSpace(voucherStatus))
                query = query.Where(v => v.VoucherStatus == voucherStatus);


            return await query
                .AsNoTracking()
                .OrderByDescending(v => v.VoucherDate)
                .ToListAsync<object>();
        }

        public async Task<object> GetCollectionStatusAsync(int companyId, int userId, DateTime? fromDate, DateTime? toDate, int? propertyId, int? unitId, string? voucherStatus, string? search, int pageNumber = 1, int pageSize = 10)
        {
            List<int>? userPropertyIds = null;

            userPropertyIds = await _context.UserMapping
                .Where(m => m.UserID == userId && m.IsActive)
                .Select(m => m.PropID)
                .ToListAsync();

            DateTime from = (fromDate ?? DateTime.Today).Date;
            DateTime to = (toDate ?? DateTime.Today).Date;

            var tenantQuery = _context.TenantAssignemnts
                .AsNoTracking()
                .Where(t =>
                    t.companyID == companyId &&
                    t.isActive &&
                    !t.isClosure &&
                    (userPropertyIds == null || userPropertyIds.Contains(t.propID)));

            if (propertyId.HasValue)
            {
                tenantQuery = tenantQuery.Where(t => t.Unit.PropID == propertyId.Value);
            }

            if (unitId.HasValue)
            {
                tenantQuery = tenantQuery.Where(t => t.unitID == unitId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                tenantQuery = tenantQuery.Where(t =>
                    t.Tenant.tenantName.Contains(search) ||
                    t.Unit.UnitName.Contains(search));
            }

            var tenants = await tenantQuery
                .Select(t => new
                {
                    t.tenantID,
                    TenantName = t.Tenant.tenantName,
                    t.unitID,
                    UnitName = t.Unit.UnitName,
                    PropertyId = t.Unit.PropID,
                    PropertyName = t.Unit.Property.propertyName,
                    propertyPrefix = t.Unit.Property.propertyPrefix,
                    t.collectionType,
                    t.rentCollection,
                    t.rentAmt,
                    t.agreementStartDate,
                    t.agreementEndDate
                })
                .ToListAsync();

            if (!tenants.Any())
            {
                return new
                {
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0,
                    Data = new List<object>()
                };
            }

            var tenantIds = tenants.Select(x => x.tenantID).Distinct().ToList();
            var unitIds = tenants.Select(x => x.unitID).Distinct().ToList();

            var vouchers = await _context.Vouchers
                .AsNoTracking()
                .Where(v => tenantIds.Contains(v.CrID) && v.VoucherType == "Pay Rent" && !v.Cancelled)
                .Select(v => new
                {
                    v.VoucherID,
                    v.VoucherNo,
                    v.CrID,
                    v.VoucherDate,
                    v.RentMonth,
                    v.RentYear,
                    v.VoucherStatus
                })
                .ToListAsync();

            var voucherLookup = vouchers
                .GroupBy(v => new { v.CrID, v.RentYear, v.RentMonth })
                .ToDictionary(
                    g => $"{g.Key.CrID}_{g.Key.RentYear}_{g.Key.RentMonth}",
                    g => g.First()
                );

            var chequeRegisters = await _context.TenantChequeRegisters
                .AsNoTracking()
                .Where(x => tenantIds.Contains(x.tenantID) && x.active)
                .Select(x => new
                {
                    x.chequeRegisterId,
                    x.propID,
                    x.unitID,
                    x.tenantID,
                    x.chequeNo,
                    x.chequeUrl,
                    x.chequeDate,
                    x.issueBank,
                    x.amount,
                    x.status
                })
                .ToListAsync();

            var chequeLookup = chequeRegisters
                .Where(x => x.chequeDate.HasValue)
                .GroupBy(x => new { x.tenantID, Year = x.chequeDate.Value.Year, Month = x.chequeDate.Value.Month })
                .ToDictionary(
                    g => $"{g.Key.tenantID}_{g.Key.Year}_{g.Key.Month}",
                    g => g.First()
                );

            var voucherIds = vouchers.Select(v => v.VoucherID).Distinct().ToList();

            var voucherChargeDetails = await (
                from vd in _context.VoucherDetails.AsNoTracking()
                join charge in _context.Charges.AsNoTracking()
                    on vd.chargeId equals charge.chargeID
                where voucherIds.Contains(vd.voucherId)
                select new
                {
                    vd.voucherId,
                    vd.chargeId,
                    ChargeName = charge.chargeName,
                    Amount = vd.amount,
                    IsVariable = charge.isVariable
                }
            ).ToListAsync();

            var voucherChargeDetailLookup = voucherChargeDetails
                .GroupBy(x => x.voucherId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var voucherInfoLookup = vouchers
                .ToDictionary(v => v.VoucherID, v => new { v.CrID, v.RentYear, v.RentMonth });

            var voucherChargeHistory = (
                from vd in voucherChargeDetails
                where voucherInfoLookup.ContainsKey(vd.voucherId)
                let info = voucherInfoLookup[vd.voucherId]
                select new
                {
                    info.CrID,
                    vd.chargeId,
                    info.RentYear,
                    info.RentMonth
                }
            ).ToList();

            var lastChargedLookup = voucherChargeHistory
                .GroupBy(x => $"{x.CrID}_{x.chargeId}")
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var latest = g.OrderByDescending(x => x.RentYear)
                                      .ThenByDescending(x => x.RentMonth)
                                      .First();
                        return (RentYear: latest.RentYear, RentMonth: latest.RentMonth);
                    }
                );

            var allCharges = await (
                from mapping in _context.UnitChargesMappings.AsNoTracking()
                join charge in _context.Charges.AsNoTracking()
                    on mapping.chargeID equals charge.chargeID
                where unitIds.Contains(mapping.unitID)
                      && mapping.isActive
                      && charge.isActive
                select new
                {
                    mapping.unitID,
                    ChargeID = charge.chargeID,
                    ChargeName = charge.chargeName,
                    Amount = mapping.amount,
                    IsVariable = charge.isVariable,
                    Frequency = mapping.frequency
                }
            ).ToListAsync();

            var chargeLookup = allCharges
                .GroupBy(x => x.unitID)
                .ToDictionary(g => g.Key, g => g.ToList());

            var variableChargeUnits = await (
                from mapping in _context.UnitChargesMappings.AsNoTracking()
                join charge in _context.Charges.AsNoTracking()
                    on mapping.chargeID equals charge.chargeID
                where unitIds.Contains(mapping.unitID)
                      && mapping.isActive
                      && charge.isActive
                      && charge.isVariable
                select mapping.unitID
            ).Distinct().ToListAsync();

            var variableChargeSet = variableChargeUnits.ToHashSet();

            var result = new List<dynamic>();

            foreach (var tenant in tenants)
            {
                int intervalMonths = tenant.collectionType?.ToLower() switch
                {
                    "monthly" => 1,
                    "2months" => 2,
                    "quaterly" => 3,
                    "6months" => 6,
                    "yearly" => 12,
                    _ => 1
                };

                int dueDay = tenant.rentCollection <= 0 ? 1 : tenant.rentCollection;

                DateTime currentMonth = new DateTime(
                    tenant.agreementStartDate.Year,
                    tenant.agreementStartDate.Month,
                    1).AddMonths(intervalMonths);

                DateTime chargeAnchorDate = currentMonth;

                var chargeProgress = new Dictionary<int, int>(); 

                while (currentMonth <= tenant.agreementEndDate.Date)
                {
                    int validDay = Math.Min(dueDay, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month));
                    var dueDate = new DateTime(currentMonth.Year, currentMonth.Month, validDay);

                    if (dueDate < tenant.agreementStartDate.Date)
                    {
                        currentMonth = currentMonth.AddMonths(intervalMonths);
                        continue;
                    }

                    if (dueDate >= from && dueDate <= to)
                    {
                        var key = $"{tenant.tenantID}_{dueDate.Year}_{dueDate.Month}";

                        voucherLookup.TryGetValue(key, out var voucher);
                        chequeLookup.TryGetValue(key, out var cheque);

                        bool hasVariableCharge = variableChargeSet.Contains(tenant.unitID);

                        string status = "Pending";

                        if (voucher != null)
                        {
                            status = string.IsNullOrWhiteSpace(voucher.VoucherStatus)
                                ? "Initiated"
                                : voucher.VoucherStatus;
                        }
                        else if (cheque != null)
                        {
                            status = !string.IsNullOrWhiteSpace(cheque.status)
                                ? cheque.status
                                : "Cheque Submitted";
                        }
                        else
                        {
                            status = hasVariableCharge ? "Not Initiated" : "Pending";
                        }

                        if (!string.IsNullOrWhiteSpace(voucherStatus))
                        {
                            if (voucherStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                            {
                                if (status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                                {
                                    currentMonth = currentMonth.AddMonths(intervalMonths);
                                    continue;
                                }
                            }
                            else
                            {
                                if (!status.Equals(voucherStatus, StringComparison.OrdinalIgnoreCase))
                                {
                                    currentMonth = currentMonth.AddMonths(intervalMonths);
                                    continue;
                                }
                            }
                        }

                        var chargeItems = new List<object>();

                        if (voucher != null)
                        {
                            if (voucherChargeDetailLookup.TryGetValue(voucher.VoucherID, out var invoicedCharges))
                            {
                                foreach (var vc in invoicedCharges)
                                {
                                    chargeItems.Add(new
                                    {
                                        ChargeID = vc.chargeId,
                                        ChargeName = vc.ChargeName,
                                        Amount = vc.Amount,
                                        IsVariable = vc.IsVariable
                                    });
                                }
                            }

          
                            if (invoicedCharges != null)
                            {
                                foreach (var vc in invoicedCharges)
                                {
                                    int dueTotalMonths = dueDate.Year * 12 + dueDate.Month;
                                    chargeProgress[vc.chargeId] = dueTotalMonths;
                                }
                            }
                        }
                        else if (chargeLookup.ContainsKey(tenant.unitID))
                        {
                            foreach (var charge in chargeLookup[tenant.unitID])
                            {
                                int occurrences = GetChargeOccurrences(
                                    lastChargedLookup,
                                    chargeProgress,
                                    tenant.tenantID,
                                    charge.ChargeID,
                                    charge.Frequency,
                                    dueDate,
                                    chargeAnchorDate);

                                if (occurrences <= 0)
                                {             
                                    continue;
                                }

                                chargeItems.Add(new
                                {
                                    charge.ChargeID,
                                    charge.ChargeName,
                                    Amount = charge.Amount,
                                    charge.IsVariable,
                                    Occurrences = occurrences
                                });
                            }
                        }

                        decimal fixedChargeAmount = chargeItems
                            .Where(x => !((dynamic)x).IsVariable)
                            .Sum(x => (decimal)((dynamic)x).Amount);

                        decimal variableChargeAmount = chargeItems
                            .Where(x => ((dynamic)x).IsVariable)
                            .Sum(x => (decimal)((dynamic)x).Amount);

                        decimal totalChargeAmount = fixedChargeAmount + variableChargeAmount;
                        decimal totalRentAmount = (tenant.rentAmt * intervalMonths) + (totalChargeAmount * intervalMonths);


                        result.Add(new
                        {
                            VoucherID = voucher?.VoucherID ?? 0,
                            VoucherNo = tenant.propertyPrefix + voucher?.VoucherNo,
                            VoucherStatus = voucher?.VoucherStatus,
                            ChequeSubmitted = cheque != null,
                            ChequeRegisterId = cheque?.chequeRegisterId,
                            ChequeNo = cheque?.chequeNo,
                            ChequeUrl = cheque?.chequeUrl,
                            ChequeDate = cheque?.chequeDate,
                            ChequeBank = cheque?.issueBank,
                            ChequeAmount = cheque?.amount,
                            ChequeStatus = cheque?.status,
                            tenant.tenantID,
                            tenant.TenantName,
                            tenant.unitID,
                            tenant.UnitName,
                            tenant.PropertyId,
                            tenant.PropertyName,
                            tenant.propertyPrefix,
                            RentDueDate = dueDate,
                            RentAmount = totalRentAmount,
                            BaseRentAmount = tenant.rentAmt,
                            ChargeAmount = (totalChargeAmount * intervalMonths),
                            FixedChargeAmount = (fixedChargeAmount * intervalMonths),
                            VariableChargeAmount = (variableChargeAmount * intervalMonths),
                            Charges = chargeItems,
                            Status = status
                        });
                    }

                    currentMonth = currentMonth.AddMonths(intervalMonths);
                }
            }

            var orderedResult = result
                .OrderBy(x => x.RentDueDate)
                .ThenBy(x => x.TenantName)
                .ToList();

            var totalRecords = orderedResult.Count;

            var pagedData = orderedResult
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            decimal totalRent = orderedResult.Sum(x => (decimal)x.RentAmount);

            return new
            {
                TotalRecords = totalRecords,
                TotalRent = totalRent,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = pagedData
            };
        }

      
        private int GetChargeOccurrences(
            Dictionary<string, (int RentYear, int RentMonth)> lastChargedLookup,
            Dictionary<int, int> chargeProgress,
            int tenantId,
            int chargeId,
            string frequency,
            DateTime dueDate,
            DateTime chargeAnchorDate)
        {
            int intervalMonths = frequency?.ToLower() switch
            {
                "monthly" => 1,
                "2months" => 2,
                "quaterly" => 3,
                "6months" => 6,
                "yearly" => 12,
                _ => 1
            };

            int dueTotalMonths = dueDate.Year * 12 + dueDate.Month;

            int baselineTotalMonths;

            if (chargeProgress.TryGetValue(chargeId, out var runtimeLast))
            {
                baselineTotalMonths = runtimeLast;
            }
            else
            {
                string key = $"{tenantId}_{chargeId}";

                if (lastChargedLookup.TryGetValue(key, out var lastInvoiced))
                {
                    baselineTotalMonths = lastInvoiced.RentYear * 12 + lastInvoiced.RentMonth;
                }
                else
                {
                    int anchorTotalMonths = chargeAnchorDate.Year * 12 + chargeAnchorDate.Month;
                    baselineTotalMonths = anchorTotalMonths - intervalMonths;
                }
            }

            int monthsElapsed = dueTotalMonths - baselineTotalMonths;

            if (monthsElapsed <= 0)
            {
                return 0;
            }

            int occurrences = monthsElapsed / intervalMonths;

            if (occurrences <= 0)
            {
                return 0;
            }
            chargeProgress[chargeId] = baselineTotalMonths + (occurrences * intervalMonths);

            return occurrences;
        }


        public async Task<object?> GetVoucherByIdAsync(int id)
        {
            var query = from v in _context.Vouchers
                        where v.VoucherID == id && v.VoucherType == "Expense Voucher"
                        join dr in _context.Ledgers on v.DrID equals dr.ledgerID
                        join cr in _context.Ledgers on v.CrID equals cr.ledgerID
                        select new
                        {
                            v.VoucherID,
                            v.VoucherNo,
                            v.VoucherDate,
                            v.VoucherType,
                            v.Amount,
                            v.RefNo,
                            v.Remarks,
                            v.IssueingBank,
                            v.ChequeNo,
                            v.Cancelled,
                            v.CrAmount,
                            v.IsReconcil,
                            v.ChequeStatus,
                            v.ReconcilDate,
                            v.CreatedOn,
                            v.CreatedBy,
                            v.ModificationBy,
                            v.isActive,
                            DrID = dr.ledgerID,
                            DrName = dr.ledgerName,
                            CrID = cr.ledgerID,
                            CrName = cr.ledgerName
                        };

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<XRS_Voucher> CreateVoucherAsync(VoucherDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var indirectExpensesGroup = await _context.AccountGroups
                    .FirstOrDefaultAsync(g => g.groupName == "INDIRECT EXPENSES" && g.companyID == dto.CompanyID);

                if (indirectExpensesGroup == null)
                    throw new Exception("Indirect Expenses account group not found.");

                var drLedger = await _context.Ledgers
                    .FirstOrDefaultAsync(l => l.ledgerID == dto.DrID && l.companyID == dto.CompanyID);

                if (drLedger == null)
                    throw new Exception($"Ledger '{dto.DrID}' not found.");

                var now = DateTime.Now;

             
                int nextVoucherNo = 1;
                var lastVoucher = await _context.Vouchers
                    .Where(v => v.CompanyID == dto.CompanyID)
                    .OrderByDescending(v => v.VoucherNo)
                    .FirstOrDefaultAsync();

                if (lastVoucher != null)
                {
                    if (int.TryParse(lastVoucher.VoucherNo, out int lastNo))
                    {
                        nextVoucherNo = lastNo + 1;
                    }
                }

                string voucherNo = nextVoucherNo.ToString("D5"); 

                var voucher = new XRS_Voucher
                {
                    unitID = dto.UnitID,
                    CompanyID = dto.CompanyID,
                    PropID = dto.PropID,
                    VoucherNo = voucherNo,
                    VoucherDate = dto.VoucherDate,
                    VoucherType = dto.VoucherType,
                    DrID = drLedger.ledgerID,
                    CrID = dto.CrID,
                    Amount = dto.Amount,
                    RefNo = dto.RefNo,
                    Remarks = dto.Remarks,
                    IssueingBank = dto.IssuingBank,
                    ChequeNo = dto.ChequeNo,
                    Cancelled = dto.Cancelled,
                    CrAmount = dto.CrAmount ?? 0,
                    IsReconcil = dto.IsReconcil,
                    ChequeStatus = dto.ChequeStatus,
                    ReconcilDate = dto.ReconcilDate,
                    CreatedOn = now,
                    ModifiedOn = now,
                    CreatedBy = dto.CreatedBy ?? "System",
                    ModificationBy = dto.ModificationBy,
                    VoucherStatus = "Success",
                    isActive = dto.IsActive
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                var debitEntry = new XRS_Accounts
                {
                    companyID = dto.CompanyID,
                    VoucherId = voucher.VoucherID,
                    GroupId = indirectExpensesGroup.groupID,
                    invType = voucher.VoucherType,
                    invNo = voucher.VoucherNo,
                    invDate = voucher.VoucherDate,
                    ledgerDr = drLedger.ledgerID,
                    ledgerCr = dto.CrID,
                    amountDr = 0,
                    amountCr = voucher.Amount,
                    remarks = "Indirect Expenses Voucher",
                    createdOn = now,
                    createdBy = dto.CreatedBy ?? "System",
                    modifiedOn = now,
                    modifiedBy = dto.CreatedBy ?? "System",
                    isActive = true
                };

                var creditEntry = new XRS_Accounts
                {
                    companyID = dto.CompanyID,
                    VoucherId = voucher.VoucherID,
                    GroupId = indirectExpensesGroup.groupID,
                    invType = voucher.VoucherType,
                    invNo = voucher.VoucherNo,
                    invDate = voucher.VoucherDate,
                    ledgerDr = dto.CrID,
                    ledgerCr = drLedger.ledgerID,
                    amountDr = voucher.Amount,
                    amountCr = 0,
                    remarks = "Indirect Expenses Voucher",
                    createdOn = now,
                    createdBy = dto.CreatedBy ?? "System",
                    modifiedOn = now,
                    modifiedBy = dto.CreatedBy ?? "System",
                    isActive = true
                };

                _context.Accounts.AddRange(debitEntry, creditEntry);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return voucher;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<XRS_Voucher?> UpdateVoucherAsync(int voucherId, VoucherDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherID == voucherId);
                if (voucher == null)
                    return null;


                var drLedger = await _context.Ledgers
                               .FirstOrDefaultAsync(g => g.ledgerID == dto.DrID && g.companyID == dto.CompanyID);

                voucher.VoucherNo = voucher.VoucherNo;
                voucher.VoucherDate = dto.VoucherDate;
                voucher.VoucherType = dto.VoucherType;
                voucher.DrID = drLedger.ledgerID;
                voucher.CrID = dto.CrID;
                voucher.Amount = dto.Amount;
                voucher.RefNo = dto.RefNo;
                voucher.Remarks = dto.Remarks;
                voucher.IssueingBank = dto.IssuingBank;
                voucher.ChequeNo = dto.ChequeNo;
                voucher.Cancelled = dto.Cancelled;
                voucher.CrAmount = dto.CrAmount ?? voucher.CrAmount;
                voucher.IsReconcil = dto.IsReconcil;
                voucher.ChequeStatus = dto.ChequeStatus;
                voucher.ReconcilDate = dto.ReconcilDate;
                voucher.ModificationBy = dto.ModificationBy ?? "System";
                voucher.ModifiedOn = DateTime.Now;

                var accounts = await _context.Accounts
                    .Where(a => a.VoucherId == voucher.VoucherID)
                    .ToListAsync();


                var debitEntry = accounts.FirstOrDefault(a => a.amountDr > 0);
                if (debitEntry != null)
                {
                    debitEntry.ledgerDr = drLedger.ledgerID;
                    debitEntry.ledgerCr = dto.CrID;
                    debitEntry.amountDr = dto.Amount;
                    debitEntry.amountCr = 0;
                    debitEntry.remarks = "Indirect Expenses Debit Entry (Updated)";
                    debitEntry.modifiedOn = DateTime.Now;
                    debitEntry.modifiedBy = dto.ModificationBy ?? "System";
                }

    
                var creditEntry = accounts.FirstOrDefault(a => a.amountCr > 0);
                if (creditEntry != null)
                {
                    creditEntry.ledgerDr = dto.CrID;
                    creditEntry.ledgerCr = drLedger.ledgerID;
                    creditEntry.amountDr = 0;
                    creditEntry.amountCr = dto.Amount;
                    creditEntry.remarks = "Cash/Bank Credit Entry (Updated)";
                    creditEntry.modifiedOn = DateTime.Now;
                    creditEntry.modifiedBy = dto.ModificationBy ?? "System";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return voucher;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<XRS_Voucher> UpdatePaymentVoucherAsync(UpdatePaymentVoucherDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.Now;

                bool isManualRef =
                    string.IsNullOrWhiteSpace(dto.RefNo) ||
                    dto.RefNo.Trim().Equals("MANUAL", StringComparison.OrdinalIgnoreCase);

                var cashLedger = await _context.Ledgers
                    .FirstOrDefaultAsync(l =>
                        l.ledgerCode == "Cash" &&
                        l.companyID == dto.companyId);

                var bankLedger = await _context.Ledgers
                    .FirstOrDefaultAsync(l =>
                        l.ledgerCode == "Bank" &&
                        l.companyID == dto.companyId);

                if (cashLedger == null)
                    throw new Exception("Cash ledger not found.");

                if (bankLedger == null)
                    throw new Exception("Bank ledger not found.");

                var paymentLedger = isManualRef ? cashLedger : bankLedger;

                var indirectIncomeGroup = await _context.AccountGroups
                    .FirstOrDefaultAsync(g =>
                        g.groupName == "INDIRECT INCOME" &&
                        g.companyID == dto.companyId);

                if (indirectIncomeGroup == null)
                    throw new Exception("Indirect Income group not found.");

                XRS_Voucher? voucher;

           
                if (dto.voucherId == 0 || dto.voucherId == null)
                {
                    var lastVoucher = await _context.Vouchers
                        .Where(v => v.CompanyID == dto.companyId && v.VoucherType == "Pay Rent")
                        .OrderByDescending(v => v.VoucherID)
                        .FirstOrDefaultAsync();

                    string newVoucherNo = GenerateVoucherNo(lastVoucher?.VoucherNo);

                    voucher = new XRS_Voucher
                    {
                        CompanyID = dto.companyId,
                        VoucherNo = newVoucherNo,
                        VoucherType = "Pay Rent",
                        VoucherDate = dto.VoucherDate,
                        RefNo = dto.RefNo,
                        Remarks = dto.Remarks,
                        IssueingBank = dto.IssuingBank,
                        ChequeNo = dto.ChequeNo,
                        Cancelled = dto.Cancelled,
                        IsReconcil = dto.IsReconcil,
                        ChequeStatus = dto.ChequeStatus,
                        ReconcilDate = dto.ReconcilDate,
                        VoucherStatus = dto.VoucherStatus,
                        DrID = paymentLedger.ledgerID,
                        CrID = dto.tenantId,   
                        Amount = dto.amount,
                        unitID = dto.unitId,
                        PropID = dto.propId,
                        RentMonth = dto.rentMonth,
                        RentYear = dto.rentYear,
                        isActive = true,
                        CreatedOn = now,
                        CreatedBy = dto.ModificationBy ?? "System",
                        ModifiedOn = now,
                        ModificationBy = dto.ModificationBy ?? "System"
                    };

                    _context.Vouchers.Add(voucher);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    voucher = await _context.Vouchers
                        .FirstOrDefaultAsync(v => v.VoucherID == dto.voucherId);

                    if (voucher == null)
                        throw new Exception("Voucher not found.");

                    voucher.VoucherDate = dto.VoucherDate;
                    voucher.RefNo = dto.RefNo;
                    voucher.Remarks = dto.Remarks;
                    voucher.IssueingBank = dto.IssuingBank;
                    voucher.ChequeNo = dto.ChequeNo;
                    voucher.Cancelled = dto.Cancelled;
                    voucher.IsReconcil = dto.IsReconcil;
                    voucher.ChequeStatus = dto.ChequeStatus;
                    voucher.ReconcilDate = dto.ReconcilDate;
                    voucher.VoucherStatus = dto.VoucherStatus;
                    voucher.ModifiedOn = now;
                    voucher.ModificationBy = dto.ModificationBy ?? "System";
                    voucher.DrID = paymentLedger.ledgerID;

                    await _context.SaveChangesAsync();
        
                    var oldAccounts = await _context.Accounts
                        .Where(a => a.VoucherId == voucher.VoucherID)
                        .ToListAsync();

                    if (oldAccounts.Any())
                    {
                        _context.Accounts.RemoveRange(oldAccounts);
                        await _context.SaveChangesAsync();
                    }
                }
                var debitEntry = new XRS_Accounts
                {
                    companyID = voucher.CompanyID,
                    VoucherId = voucher.VoucherID,
                    GroupId = indirectIncomeGroup.groupID,
                    invType = voucher.VoucherType,
                    invNo = voucher.VoucherNo,
                    invDate = voucher.VoucherDate,
                    ledgerDr = paymentLedger.ledgerID,
                    ledgerCr = voucher.CrID,
                    amountDr = voucher.Amount,
                    amountCr = 0,
                    remarks = isManualRef
                        ? "Indirect Income - Cash Debit"
                        : "Indirect Income - Bank Debit",
                    createdOn = now,
                    createdBy = dto.ModificationBy ?? "System",
                    modifiedOn = now,
                    modifiedBy = dto.ModificationBy ?? "System",
                    isActive = true
                };

                var creditEntry = new XRS_Accounts
                {
                    companyID = voucher.CompanyID,
                    VoucherId = voucher.VoucherID,
                    GroupId = indirectIncomeGroup.groupID,
                    invType = voucher.VoucherType,
                    invNo = voucher.VoucherNo,
                    invDate = voucher.VoucherDate,
                    ledgerDr = voucher.CrID,
                    ledgerCr = paymentLedger.ledgerID,
                    amountDr = 0,
                    amountCr = voucher.Amount,
                    remarks = "Indirect Income - Credit",
                    createdOn = now,
                    createdBy = dto.ModificationBy ?? "System",
                    modifiedOn = now,
                    modifiedBy = dto.ModificationBy ?? "System",
                    isActive = true
                };

                _context.Accounts.AddRange(debitEntry, creditEntry);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return voucher;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private string GenerateVoucherNo(string? lastVoucherNo)
        {
            if (string.IsNullOrWhiteSpace(lastVoucherNo))
                return "PAY-0001";

            var parts = lastVoucherNo.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                return $"{parts[0]}-{(num + 1):D4}";

            return "PAY-0001";
        }

        public async Task<XRS_Voucher> CreateIntiateAsync(VoucherCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var lastVoucherNo = await _context.Vouchers
                   .Where(v => v.CompanyID == request.CompanyID
                            && v.PropID == request.PropID
                            && v.unitID == request.UnitID)
                   .OrderByDescending(v => v.VoucherID) 
                   .Select(v => v.VoucherNo)
                   .FirstOrDefaultAsync();

                int newVoucherNo = 1; 
                if (!string.IsNullOrEmpty(lastVoucherNo) && int.TryParse(lastVoucherNo, out int parsedNo))
                {
                    newVoucherNo = parsedNo + 1;
                }
                var voucher = new XRS_Voucher
                {
                    unitID = request.UnitID,
                    CompanyID = request.CompanyID,
                    PropID = request.PropID,
                    VoucherNo = newVoucherNo.ToString(),
                    VoucherDate = request.VoucherDate,
                    VoucherType = request.VoucherType,
                    RentMonth =request.RentMonth,
                    RentYear = request.RentYear,    
                    DrID = 0,
                    CrID = request.CrID,
                    Amount = request.Amount,
                    RefNo = request.RefNo,
                    Remarks = request.Remarks,
                    VoucherStatus = request.VoucherStatus ?? "Initiated",
                    isActive = request.IsActive,
                    CreatedOn = DateTime.Now,
                    CreatedBy = request.createdBy,
                    ModificationBy = request.modifiedBy,
                    ModifiedOn = DateTime.Now,
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                foreach (var detail in request.VoucherDetails)
                {
                    var voucherDetail = new XRS_VoucherDetails
                    {
                        voucherId = voucher.VoucherID,
                        chargeId = detail.ChargeId,
                        amount = detail.Amount
                    };
                    _context.VoucherDetails.Add(voucherDetail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return voucher;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<XRS_Voucher> UpdateAsync(int voucherId, VoucherCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.VoucherID == voucherId);

                if (voucher == null)
                    throw new Exception("Voucher not found");
      
                voucher.VoucherDate = request.VoucherDate;
                voucher.VoucherType = request.VoucherType;
                voucher.DrID = 0;
                voucher.CrID = request.CrID;
                voucher.Amount = request.Amount;
                voucher.RefNo = request.RefNo;
                voucher.Remarks = request.Remarks;
                voucher.VoucherStatus = request.VoucherStatus ?? voucher.VoucherStatus;
                voucher.isActive = request.IsActive;
                voucher.ModifiedOn = DateTime.Now;
                voucher.ModificationBy = request.modifiedBy;

                var existingDetails = await _context.VoucherDetails
                    .Where(d => d.voucherId == voucher.VoucherID)
                    .ToListAsync();

                _context.VoucherDetails.RemoveRange(existingDetails);

                foreach (var detail in request.VoucherDetails)
                {
                    _context.VoucherDetails.Add(new XRS_VoucherDetails
                    {
                        voucherId = voucher.VoucherID,
                        chargeId = detail.ChargeId,
                        amount = detail.Amount
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return voucher;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<object> GetTenantChargesByMonthAsync(int companyId, int userId, int month, int year, int? propertyId = null, int? unitId = null, int? bedSpaceId = null, string? search = null, int pageNumber = 1, int pageSize = 25)
        {
            List<int>? userPropertyIds = null;

            userPropertyIds = await _context.UserMapping
                .Where(m => m.UserID == userId && m.IsActive)
                .Select(m => m.PropID)
                .ToListAsync();


            var tenantQuery = _context.TenantAssignemnts
                .AsNoTracking()
                .Where(t =>
                    !t.isClosure &&
                    t.companyID == companyId &&
                    (userPropertyIds == null || userPropertyIds.Contains(t.propID)));

            if (propertyId.HasValue)
            {
                tenantQuery = tenantQuery.Where(t =>
                    t.Unit.Property.PropID == propertyId.Value);
            }

            if (unitId.HasValue)
            {
                tenantQuery = tenantQuery.Where(t =>
                    t.unitID == unitId.Value);
            }

            if (bedSpaceId.HasValue)
            {
                tenantQuery = tenantQuery.Where(t =>
                    t.bedSpaceID == bedSpaceId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                tenantQuery = tenantQuery.Where(t =>
                    t.Tenant.tenantName.Contains(search) ||
                    t.Unit.UnitName.Contains(search));
            }

            var tenants = await tenantQuery
                .Select(t => new
                {
                    t.tenantID,
                    TenantName = t.Tenant.tenantName,
                    t.unitID,
                    UnitName = t.Unit.UnitName,
                    PropertyName = t.Unit.Property.propertyName,
                    PropertyId = t.Unit.Property.PropID,
                    BedSpaceName = t.BedSpace != null ? t.BedSpace.bedSpaceName : null,
                    t.agreementStartDate,
                    t.rentCollection,
                    t.collectionType,
                    t.rentAmt
                })
                .ToListAsync();

            if (!tenants.Any())
            {
                return new
                {
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0,
                    Data = new List<object>()
                };
            }

            var tenantIds = tenants.Select(x => x.tenantID).Distinct().ToList();
            var unitIds = tenants.Select(x => x.unitID).Distinct().ToList();


            var vouchers = await _context.Vouchers
                .AsNoTracking()
                .Where(v =>
                    tenantIds.Contains(v.CrID) &&
                    v.VoucherType == "Pay Rent" &&
                    v.isActive &&
                    (
                        v.RentYear < year ||
                        (
                            v.RentYear == year &&
                            v.RentMonth <= month
                        )
                    ))
                .Select(v => new
                {
                    v.VoucherID,
                    v.CrID,
                    v.VoucherDate,
                    v.RentMonth,
                    v.RentYear,
                    v.VoucherStatus
                })
                .ToListAsync();

            var voucherIds = vouchers.Select(v => v.VoucherID).Distinct().ToList();


            var voucherDetails = await _context.VoucherDetails
                .AsNoTracking()
                .Where(v => voucherIds.Contains(v.voucherId))
                .Select(d => new
                {
                    d.voucherId,
                    d.chargeId,
                    d.amount,
                    ChargeName = d.Charge.chargeName,
                    IsVariable = d.Charge.isVariable,
                    IsRentCharge = d.Charge.chargeName == "Rent"
                })
                .ToListAsync();


            var voucherDetailLookup = voucherDetails
                .GroupBy(x => x.voucherId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new ChargeDto
                    {
                        ChargeId = x.chargeId,
                        ChargeName = x.ChargeName,
                        ChargeAmount = x.amount,
                        IsVariable = x.IsVariable
                    }).ToList()
                );


            var voucherRentLookup = voucherDetails
                .Where(x => x.IsRentCharge)
                .GroupBy(x => x.voucherId)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().amount
                );

            var voucherCrIdLookup = vouchers
                .ToDictionary(v => v.VoucherID, v => v.CrID);


            var voucherChargeHistory = (
                from vd in voucherDetails
                where voucherCrIdLookup.ContainsKey(vd.voucherId)
                let v = vouchers.First(x => x.VoucherID == vd.voucherId)
                select new
                {
                    CrID = v.CrID,
                    vd.chargeId,
                    v.RentYear,
                    v.RentMonth,
                    vd.amount
                }
            ).ToList();

            var lastChargedLookup = voucherChargeHistory
                .GroupBy(x => $"{x.CrID}_{x.chargeId}")
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var latest = g.OrderByDescending(x => x.RentYear)
                                      .ThenByDescending(x => x.RentMonth)
                                      .First();
                        return (RentYear: latest.RentYear, RentMonth: latest.RentMonth, Amount: latest.amount);
                    }
                );
            var unitCharges = await (
                from mapping in _context.UnitChargesMappings.AsNoTracking()
                join charge in _context.Charges.AsNoTracking()
                    on mapping.chargeID equals charge.chargeID
                where unitIds.Contains(mapping.unitID)
                      && mapping.isActive
                      && charge.isActive
                select new
                {
                    mapping.unitID,
                    Charge = new ChargeDto
                    {
                        ChargeId = charge.chargeID,
                        ChargeName = charge.chargeName,
                        ChargeAmount = charge.chargeAmt,
                        IsVariable = charge.isVariable
                    },
                    Frequency = mapping.frequency
                }
            ).ToListAsync();

            var unitChargeLookup = unitCharges
                .GroupBy(x => x.unitID)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x =>
                    {
                        x.Charge.Frequency = x.Frequency;
                        return x.Charge;
                    }).ToList()
                );

            var voucherLookup = vouchers
                .GroupBy(v => new { v.CrID, v.RentYear, v.RentMonth })
                .ToDictionary(
                    g => $"{g.Key.CrID}_{g.Key.RentYear}_{g.Key.RentMonth}",
                    g => g.First()
                );

            var result = new List<dynamic>(5000);

            foreach (var tenant in tenants)
            {
                var dueDates = GenerateDueDates(
                    tenant.agreementStartDate,
                    tenant.rentCollection,
                    tenant.collectionType,
                    month,
                    year
                );

                int chargeIntervalAnchorMonths = tenant.collectionType?.ToLower() switch
                {
                    "monthly" => 1,
                    "2months" => 2,
                    "quaterly" => 3,
                    "6months" => 6,
                    "yearly" => 12,
                    _ => 1
                };

                DateTime chargeAnchorDate = new DateTime(
                    tenant.agreementStartDate.Year,
                    tenant.agreementStartDate.Month,
                    1).AddMonths(chargeIntervalAnchorMonths);

                foreach (var nextDueDate in dueDates)
                {
                    var key = $"{tenant.tenantID}_{nextDueDate.Year}_{nextDueDate.Month}";

                    voucherLookup.TryGetValue(key, out var voucher);

                    string status;
                    List<ChargeDto> variableCharges;
                    List<ChargeDto> fixedCharges;
                    decimal totalCharges;
                    decimal rentAmount;

                    if (voucher != null)
                    {
                        status = voucher.VoucherStatus ?? "Initiated";

                        var details = voucherDetailLookup.TryGetValue(
                            voucher.VoucherID,
                            out var voucherChargeList)
                            ? voucherChargeList
                            : new List<ChargeDto>();


                        if (voucherRentLookup.TryGetValue(voucher.VoucherID, out var voucherRentAmount))
                        {
                            rentAmount = voucherRentAmount;
                        }
                        else
                        {
                            rentAmount = tenant.rentAmt;
                        }

                        variableCharges = details.Where(c => c.IsVariable && c.ChargeName != "Rent").ToList();
                        fixedCharges = details.Where(c => !c.IsVariable && c.ChargeName != "Rent").ToList();
                        totalCharges = rentAmount + variableCharges.Sum(c => c.ChargeAmount) + fixedCharges.Sum(c => c.ChargeAmount);
                    }
                    else
                    {
                        status = "Not Initiated";

                        var charges = unitChargeLookup.TryGetValue(
                            tenant.unitID,
                            out var unitChargeList)
                            ? unitChargeList.Select(c => new ChargeDto
                            {
                                ChargeId = c.ChargeId,
                                ChargeName = c.ChargeName,
                                ChargeAmount = c.ChargeAmount,
                                IsVariable = c.IsVariable,
                                Frequency = c.Frequency
                            }).ToList()
                            : new List<ChargeDto>();

                        charges = charges
                            .Where(c => IsChargeDue(
                                lastChargedLookup,
                                tenant.tenantID,
                                c.ChargeId,
                                c.Frequency,
                                nextDueDate.Year,
                                nextDueDate.Month,
                                chargeAnchorDate))
                            .ToList();

                
                        foreach (var c in charges.Where(c => c.IsVariable))
                        {
                            var lastKey = $"{tenant.tenantID}_{c.ChargeId}";
                            if (lastChargedLookup.TryGetValue(lastKey, out var lastCharge))
                            {
                                c.ChargeAmount = lastCharge.Amount;
                            }
                        }

                        variableCharges = charges.Where(c => c.IsVariable).ToList();
                        fixedCharges = charges.Where(c => !c.IsVariable).ToList();

                        rentAmount = tenant.rentAmt;
                        totalCharges = rentAmount + variableCharges.Sum(c => c.ChargeAmount) + fixedCharges.Sum(c => c.ChargeAmount);
                    }
                    if (rentAmount == 0 && variableCharges.Sum(c => c.ChargeAmount) == 0 && fixedCharges.Sum(c => c.ChargeAmount) > 0)
                    {
                       //No row
                    }
                    else
                    if (rentAmount > 0 && variableCharges.Sum(c => c.ChargeAmount) == 0 && fixedCharges.Sum(c => c.ChargeAmount) == 0)
                    {
                       //No row
                    }
                    else
                    if (rentAmount > 0 && variableCharges.Sum(c => c.ChargeAmount) == 0 && fixedCharges.Sum(c => c.ChargeAmount) > 0)
                    {

                    }
                    else
                    {
                        result.Add(new
                        {
                            VoucherID = voucher != null ? voucher.VoucherID : 0,
                            tenant.tenantID,
                            tenant.TenantName,
                            tenant.PropertyId,
                            tenant.unitID,
                            tenant.UnitName,
                            tenant.PropertyName,
                            tenant.BedSpaceName,
                            tenant.rentCollection,
                            Frequency = tenant.collectionType,
                            NextRentDueDate = nextDueDate,
                            VariableCharges = variableCharges,
                            FixedCharges = fixedCharges,
                            RentAmount = rentAmount,
                            TotalCharges = totalCharges,
                            Status = status
                        });
                    }
                }
            }

            var orderedResult = result
                .OrderBy(x => x.NextRentDueDate)
                .ThenBy(x => x.TenantName)
                .ToList();

            var totalRecords = orderedResult.Count;

            var pagedData = orderedResult
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = pagedData
            };
        }

        private static int GetFrequencyMonths(string? frequency) => frequency?.ToLower() switch
        {
            "monthly" => 1,
            "2months" => 2,
            "quaterly" => 3,
            "6months" => 6,
            "yearly" => 12,
            _ => 1
        };

        private static bool IsChargeDue(Dictionary<string, (int RentYear, int RentMonth, decimal Amount)> lastChargedLookup, int tenantId, int chargeId, string? frequency, int currentYear, int currentMonth, DateTime agreementStartDate)
        {
            var key = $"{tenantId}_{chargeId}";

            int intervalMonths = GetFrequencyMonths(frequency);

            if (!lastChargedLookup.TryGetValue(key, out var last))
            {
                int monthsSinceStart =
                    ((currentYear - agreementStartDate.Year) * 12) +
                    (currentMonth - agreementStartDate.Month);

                if (monthsSinceStart < 0)
                {
                    return false;
                }

                return monthsSinceStart % intervalMonths == 0;
            }

            int monthsSinceLast =
                ((currentYear - last.RentYear) * 12) + (currentMonth - last.RentMonth);

            return monthsSinceLast > 0 && monthsSinceLast % intervalMonths == 0;
        }
     
        private List<DateTime> GenerateDueDates(DateTime agreementStart, int dueDay, string frequency, int filterMonth, int filterYear)
        {
            var dueDates = new List<DateTime>();

            if (agreementStart == DateTime.MinValue)
            {
                return dueDates;
            }

            if (dueDay <= 0)
            {
                dueDay = 1;
            }

            int intervalMonths = frequency?.ToLower() switch
            {
                "monthly" => 1,
                "2 months" => 2,
                "quarterly" => 3,
                "6 months" => 6,
                "yearly" => 12,
                _ => 1
            };

            var current = agreementStart.AddMonths(intervalMonths);

            while (
                current.Year < filterYear ||
                (
                    current.Year == filterYear &&
                    current.Month <= filterMonth
                ))
            {
                int validDay = Math.Min(
                    dueDay,
                    DateTime.DaysInMonth(
                        current.Year,
                        current.Month)
                );

                dueDates.Add(new DateTime(
                    current.Year,
                    current.Month,
                    validDay
                ));

                current = current.AddMonths(intervalMonths);
            }

            return dueDates;
        }

        public async Task<object> CreatePaymentAsync(int companyId, int tenantId, int unitId, int month, int year)
        {
            var assignment = await _context.TenantAssignemnts
                .FirstOrDefaultAsync(x =>
                    x.tenantID == tenantId &&
                    x.unitID == unitId &&
                    x.isActive);

            if (assignment == null)
                throw new Exception("No active tenancy found.");

            var existingVoucher = await _context.Vouchers
                .FirstOrDefaultAsync(v =>
                    v.CompanyID == companyId &&
                    v.CrID == tenantId &&
                    v.unitID == unitId &&
                    v.RentMonth == month &&
                    v.RentYear == year &&
                    v.VoucherStatus == "Initiated" &&
                    !v.Cancelled);

            string? razorpayKey = await _context.CompanySettings
                .Where(x => x.CompanyId == companyId && x.KeyCode == "RAZROPAY_KEY" && x.Active)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            string? razorpaySecret = await _context.CompanySettings
                .Where(x => x.CompanyId == companyId && x.KeyCode == "RAZROPAY_SECRET" && x.Active)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(razorpayKey))
                throw new Exception("Razorpay Key not configured.");

            if (string.IsNullOrWhiteSpace(razorpaySecret))
                throw new Exception("Razorpay Secret not configured.");


            var dueDate = new DateTime(year, month, 1);

            int intervalMonths = assignment.collectionType?.ToLower() switch
            {
                "monthly" => 1,
                "2months" => 2,
                "quarterly" => 3,
                "6months" => 6,
                "yearly" => 12,
                _ => 1
            };

            DateTime chargeAnchorDate = new DateTime(
                assignment.agreementStartDate.Year,
                assignment.agreementStartDate.Month,
                1).AddMonths(intervalMonths);

            var pastVouchers = await _context.Vouchers
                .AsNoTracking()
                .Where(v =>
                    v.CompanyID == companyId &&
                    v.CrID == tenantId &&
                    v.unitID == unitId &&
                    !v.Cancelled &&
                    (
                        v.RentYear < year ||
                        (v.RentYear == year && v.RentMonth <= month)
                    ))
                .Select(v => new { v.VoucherID, v.RentYear, v.RentMonth })
                .ToListAsync();

            var pastVoucherIds = pastVouchers.Select(v => v.VoucherID).Distinct().ToList();

            var pastVoucherDetails = await (
                from detail in _context.VoucherDetails.AsNoTracking()

                join charge in _context.Charges.AsNoTracking()
                    on detail.chargeId equals charge.chargeID

                where pastVoucherIds.Contains(detail.voucherId)

                select new
                {
                    detail.voucherId,
                    detail.chargeId,
                    detail.amount
                }
            ).ToListAsync();

            var pastVoucherInfoLookup = pastVouchers
                .ToDictionary(v => v.VoucherID, v => new { v.RentYear, v.RentMonth });

            var lastChargedLookup = (
                from vd in pastVoucherDetails
                where pastVoucherInfoLookup.ContainsKey(vd.voucherId)
                let info = pastVoucherInfoLookup[vd.voucherId]
                select new
                {
                    vd.chargeId,
                    info.RentYear,
                    info.RentMonth,
                    vd.amount
                }
            )
            .GroupBy(x => x.chargeId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var latest = g.OrderByDescending(x => x.RentYear)
                                  .ThenByDescending(x => x.RentMonth)
                                  .First();
                    return (RentYear: latest.RentYear, RentMonth: latest.RentMonth, Amount: latest.amount);
                }
            );

            var unitCharges = await (
                from mapping in _context.UnitChargesMappings.AsNoTracking()

                join charge in _context.Charges.AsNoTracking()
                    on mapping.chargeID equals charge.chargeID

                where mapping.unitID == unitId
                      && mapping.isActive
                      && charge.isActive

                select new
                {
                    ChargeId = mapping.chargeID,
                    ChargeAmount = mapping.amount,
                    Frequency = mapping.frequency,
                    IsVariable = charge.isVariable
                }
            ).ToListAsync();

            int GetFrequencyMonths(string? frequency) => frequency?.Trim().ToLower() switch
            {
                "monthly" => 1,
                "2months" => 2,
                "quaterly" => 3,
                "quarterly" => 3,
                "6months" => 6,
                "yearly" => 12,
                _ => 1
            };

            bool IsChargeDue(int chargeId, string? frequency)
            {
                int intervalMonthsForCharge = GetFrequencyMonths(frequency);

                if (!lastChargedLookup.TryGetValue(chargeId, out var last))
                {
                    int monthsSinceStart =
                        ((dueDate.Year - chargeAnchorDate.Year) * 12) +
                        (dueDate.Month - chargeAnchorDate.Month);

                    if (monthsSinceStart < 0)
                    {
                        return false;
                    }

                    return monthsSinceStart % intervalMonthsForCharge == 0;
                }

                int monthsSinceLast =
                    ((dueDate.Year - last.RentYear) * 12) + (dueDate.Month - last.RentMonth);

                return monthsSinceLast > 0 && monthsSinceLast % intervalMonthsForCharge == 0;
            }

            decimal applicableChargesTotal = unitCharges
                .Where(c => IsChargeDue(c.ChargeId, c.Frequency))
                .Sum(c =>
                    c.IsVariable && lastChargedLookup.TryGetValue(c.ChargeId, out var lastCharge)
                        ? lastCharge.Amount
                        : c.ChargeAmount);

            decimal amount = assignment.rentAmt + applicableChargesTotal;

            XRS_Voucher voucher;

            if (existingVoucher != null)
            {
                voucher = existingVoucher;

                if (!string.IsNullOrEmpty(voucher.transcationId))
                {
                    var status = await _paymentService.GetOrderStatusAsync(
                        voucher.transcationId,
                        razorpayKey,
                        razorpaySecret
                    );

                    if (status == "paid")
                    {
                        voucher.VoucherStatus = "PAID";
                        await _context.SaveChangesAsync();

                        throw new Exception("Already paid for this month.");
                    }


                    await InsertVoucherDetails(voucher, companyId, unitId);

                    return new
                    {
                        success = true,
                        key = razorpayKey,
                        orderId = voucher.transcationId,
                        amount = voucher.Amount,
                        voucherNo = voucher.VoucherNo,
                        status = "REUSED"
                    };
                }
                else
                {
                    voucher.Amount = amount;
                    voucher.CrAmount = amount;

                    string newOrderId = await _paymentService.CreateOrderAsync(
                        amount: voucher.Amount,
                        currency: "INR",
                        apiKey: razorpayKey,
                        apiSecret: razorpaySecret,
                        receiptNo: $"VCH_{voucher.VoucherID}",
                        customerName: $"Tenant-{tenantId}",
                        mobileNumber: "9999999999"
                    );

                    voucher.transcationId = newOrderId;
                    voucher.VoucherStatus = "Initiated";

                    await _context.SaveChangesAsync();


                    await InsertVoucherDetails(voucher, companyId, unitId);

                    return new
                    {
                        success = true,
                        key = razorpayKey,
                        orderId = newOrderId,
                        amount = voucher.Amount,
                        voucherNo = voucher.VoucherNo,
                        status = "REINITIATED"
                    };
                }
            }
            else
            {
                var lastVoucherNo = await _context.Vouchers
                    .Where(v => v.CompanyID == companyId && v.unitID == unitId)
                    .OrderByDescending(v => v.VoucherID)
                    .Select(v => v.VoucherNo)
                    .FirstOrDefaultAsync();

                int newVoucherNo = 1;
                if (!string.IsNullOrEmpty(lastVoucherNo) && int.TryParse(lastVoucherNo, out int parsedNo))
                    newVoucherNo = parsedNo + 1;

                voucher = new XRS_Voucher
                {
                    CompanyID = companyId,
                    unitID = unitId,
                    CrID = tenantId,
                    Amount = amount,
                    VoucherDate = DateTime.Now,
                    RentMonth = month,
                    RentYear = year,
                    VoucherType = "Pay Rent",
                    VoucherStatus = "Initiated",
                    Cancelled = false,
                    CrAmount = amount,
                    isActive = true,
                    CreatedOn = DateTime.Now,
                    CreatedBy = tenantId.ToString(),
                    VoucherNo = newVoucherNo.ToString("D5")
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                string orderId = await _paymentService.CreateOrderAsync(
                    amount: amount,
                    currency: "INR",
                    apiKey: razorpayKey,
                    apiSecret: razorpaySecret,
                    receiptNo: $"VCH_{voucher.VoucherID}",
                    customerName: $"Tenant-{tenantId}",
                    mobileNumber: "9999999999"
                );

                voucher.transcationId = orderId;

                await _context.SaveChangesAsync();


                await InsertVoucherDetails(voucher, companyId, unitId);

                return new
                {
                    success = true,
                    key = razorpayKey,
                    orderId = orderId,
                    amount = amount,
                    voucherNo = voucher.VoucherNo,
                    status = "CREATED_OR_UPDATED"
                };
            }
        }

        private async Task InsertVoucherDetails(XRS_Voucher voucher, int companyId, int unitId)
        {
            var chargeMappings = await _context.UnitChargesMappings
                .Where(x =>
                    x.unitID == unitId &&
                    x.companyID == companyId &&
                    x.isActive)
                .ToListAsync();

            var chargeIds = chargeMappings.Select(x => x.chargeID).ToList();

            var charges = await _context.Charges
                .Where(c =>
                    chargeIds.Contains(c.chargeID) &&
                    c.companyID == companyId &&
                    c.isActive)
                .ToListAsync();

            foreach (var charge in charges)
            {
                var exists = await _context.VoucherDetails
                    .FirstOrDefaultAsync(x =>
                        x.voucherId == voucher.VoucherID &&
                        x.chargeId == charge.chargeID);

                if (exists == null)
                {
                    _context.VoucherDetails.Add(new XRS_VoucherDetails
                    {
                        voucherId = voucher.VoucherID,
                        chargeId = charge.chargeID,
                        amount = charge.chargeAmt
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string> CheckRazorpayOrderStatusAsync(int companyId, string razorpayOrderId)
        {
            string? razorpayKey = await _context.CompanySettings
            .Where(x => x.CompanyId == companyId && x.KeyCode == "RAZROPAY_KEY" && x.Active)
            .Select(x => x.Value)
            .FirstOrDefaultAsync();

            string? razorpaySecret = await _context.CompanySettings
                .Where(x => x.CompanyId == companyId && x.KeyCode == "RAZROPAY_SECRET" && x.Active)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();


            var status =
                await _paymentService.GetOrderStatusAsync(
                    razorpayOrderId,
                    razorpayKey,
                    razorpaySecret);

            return status;
        }
    }
}
