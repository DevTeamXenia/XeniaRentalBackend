using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Common;


namespace XeniaRentalBackend.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepsitory
    {

        private readonly ApplicationDbContext _context;
        private readonly JwtHelperService _jwtHelperService;
        public DashboardRepository(ApplicationDbContext context, JwtHelperService jwtHelperService)
        {
            _context = context;
            _jwtHelperService = jwtHelperService;
        }

        public async Task<RentDashboardDto> GetRentDashboardAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            var activeAssignments = await _context.TenantAssignemnts
                .Where(t => t.companyID == companyId &&
                            t.rentDueDate >= fromDate &&
                            t.rentDueDate <= toDate &&
                            !t.isClosure)
                .ToListAsync();

            var vouchers = await _context.Vouchers
                .Where(v => v.CompanyID == companyId &&
                            v.VoucherType == "Pay Rent" &&
                            v.VoucherDate >= fromDate &&
                            v.VoucherDate <= toDate)
                .ToListAsync();

            int paidCount = 0;
            decimal totalPaidAmount = 0;
            int notPaidCount = 0;
            decimal totalNotPaidAmount = 0;

            foreach (var tenant in activeAssignments)
            {
                var voucher = vouchers.FirstOrDefault(v => v.DrID == tenant.tenantID && v.unitID == tenant.unitID);
                if (voucher != null)
                {
                    paidCount++;
                    totalPaidAmount += voucher.Amount;
                }
                else
                {
                    notPaidCount++;
                    totalNotPaidAmount += tenant.rentAmt;
                }
            }

            var occupiedPropertyIds = activeAssignments.Select(t => t.propID).Distinct().Count();
            var occupiedUnitIds = activeAssignments.Select(t => t.unitID).Distinct().Count();

            var totalPropertiesCount = await _context.Properties
                .Where(p => p.CompanyId == companyId)
                .CountAsync();

            var totalUnitsCount = await _context.Units
                .Where(u => u.CompanyId == companyId)
                .CountAsync();

            int vacantProperties = totalPropertiesCount - occupiedPropertyIds;
            int vacantUnits = totalUnitsCount - occupiedUnitIds;

            decimal averageRentPerProperty = occupiedPropertyIds > 0
                ? totalPaidAmount / occupiedPropertyIds
                : 0;

            int highRiskTenantCount = activeAssignments.Count(t =>
                !vouchers.Any(v => v.DrID == t.tenantID && v.unitID == t.unitID));

            int highRiskPropertyCount = activeAssignments
                .Where(t => !vouchers.Any(v => v.DrID == t.tenantID && v.unitID == t.unitID))
                .Select(t => t.propID)
                .Distinct()
                .Count();

            decimal occupancyRate = totalPropertiesCount > 0
                ? (decimal)occupiedPropertyIds / totalPropertiesCount * 100
                : 0;

            decimal collectionRate = (paidCount + notPaidCount) > 0
                ? (decimal)paidCount / (paidCount + notPaidCount) * 100
                : 0;

            var topPerformingProperties = vouchers
                .GroupBy(v => v.PropID)
                .Select(g => new
                {
                    PropertyId = g.Key,
                    TotalCollected = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalCollected)
                .Take(3)
                .ToList();

            int topPerformingPropertyCount = topPerformingProperties.Count;

            return new RentDashboardDto
            {
                TotalPaidCount = paidCount,
                TotalPaidAmount = totalPaidAmount,
                TotalNotPaidCount = notPaidCount,
                TotalNotPaidAmount = totalNotPaidAmount,
                TotalOccupiedProperties = occupiedPropertyIds,
                TotalOccupiedUnits = occupiedUnitIds,
                VacantProperties = vacantProperties,
                VacantUnits = vacantUnits,
                TopPerformingPropertyCount = topPerformingPropertyCount,
                AverageRentPerProperty = averageRentPerProperty,
                HighRiskTenantCount = highRiskTenantCount,
                HighRiskPropertyCount = highRiskPropertyCount,
                OccupancyRate = Math.Round(occupancyRate, 2),
                CollectionRate = Math.Round(collectionRate, 2)
            };
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRentRevenueAsync(int companyid,int year)
        {
            var vouchers = await _context.Vouchers
                .Where(v => v.VoucherType == "Pay Rent" && v.VoucherDate.Year == year && v.CompanyID == companyid)
                .ToListAsync();

            var monthlyRevenue = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var total = vouchers
                        .Where(v => v.VoucherDate.Month == month)
                        .Sum(v => v.Amount);

                    return new MonthlyRevenueDto
                    {
                        Month = new DateTime(year, month, 1).ToString("MMM"),
                        TotalRent = total
                    };
                })
                .ToList();

            return monthlyRevenue;
        }

        public async Task<TenantPaymentBannerDto> GetTenantPaymentBannerAsync(int tenantId, int companyId)
        {
            var latestPayments = await _context.Vouchers
                .Where(v => v.CompanyID == companyId && v.DrID == tenantId && v.VoucherType == "Pay Rent")
                .OrderByDescending(v => v.VoucherDate)
                .Take(3)
                .Select(v => new PaymentInfoDto
                {
                    Amount = v.Amount,
                    Date = v.VoucherDate
                })
                .ToListAsync();

    
            var upcomingPayments = await _context.TenantAssignemnts
                .Where(t => t.companyID == companyId && t.tenantID == tenantId && !t.isClosure && t.rentDueDate > DateTime.Now)
                .OrderBy(t => t.rentDueDate)
                .Select(t => new PaymentInfoDto
                {
                    Amount = t.rentAmt,
                    Date = t.rentDueDate
                })
                .Take(3)
                .ToListAsync();


            var activeBanners = await _context.Banners
                .Where(b => b.companyID == companyId && b.bannerStatus)
                .Select(b => new BannerDto
                {
                    bannerID = b.bannerID,
                    bannerName = b.bannerName,
                    bannerImage = b.bannerImage
                })
                .ToListAsync();

            var activeTexts = await _context.AdvText
             .Where(at => at.companyID == companyId && at.textStatus)
             .Select(at => new AdvTextDto
             {
                 textID = at.textID,
                 textContent = at.textContent
             })
             .ToListAsync();

            return new TenantPaymentBannerDto
            {
                LatestPayments = latestPayments,
                UpcomingPayments = upcomingPayments,
                ActiveBanners = activeBanners,
                ActiveTexts = activeTexts
            };
        }

        public async Task<TenantPaymentSummaryDto> GetTenantPaymentsAsync(int unitId)
        {
            var today = DateTime.Today;
            int tenantId = _jwtHelperService.GetCustomerId();


            var assignment = await _context.TenantAssignemnts
                .Where(x =>
                    x.tenantID == tenantId &&
                    x.unitID == unitId &&
                    x.isActive)
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                return new TenantPaymentSummaryDto
                {
                    PreviousUnpaidPayments = new List<UpcomingPaymentDto>(),
                    PreviousPaidPayments = new List<PaidPaymentDto>(),
                    NextUpcomingPayment = null
                };
            }


            var rentSchedule = new List<(DateTime DueDate, decimal Amount)>();
            var start = assignment.agreementStartDate;
            var end = assignment.agreementEndDate;

            while (start <= end)
            {
                rentSchedule.Add((start, assignment.rentAmt));
                start = start.AddMonths(1);
            }


            var paidMonths = await _context.Vouchers
                .Where(v =>
                    v.CrID == tenantId &&
                    v.unitID == unitId &&
                    v.VoucherStatus == "PAID" &&
                    !v.Cancelled)
                .Select(v => new
                {
                    v.VoucherDate.Year,
                    v.VoucherDate.Month
                })
                .ToListAsync();


            var unpaidRents = rentSchedule
                .Where(r => !paidMonths.Any(p =>
                    p.Year == r.DueDate.Year &&
                    p.Month == r.DueDate.Month))
                .OrderBy(r => r.DueDate)
                .ToList();


            var previousUnpaidPayments = unpaidRents
                .Where(x => x.DueDate < today)
                .Select(x => new UpcomingPaymentDto
                {
                    RentDueDate = x.DueDate.ToString("dd MMM yyyy"),
                    RentAmount = x.Amount
                })
                .ToList();


            var nextMonthDueDate = new DateTime(today.Year, today.Month, 1).AddMonths(1);

            UpcomingPaymentDto? nextUpcoming = null;

            if (nextMonthDueDate >= assignment.agreementStartDate &&
                nextMonthDueDate <= assignment.agreementEndDate)
            {
                nextUpcoming = new UpcomingPaymentDto
                {
                    RentDueDate = nextMonthDueDate.ToString("dd MMM yyyy"),
                    RentAmount = assignment.rentAmt
                };
            }


            var paidPayments = await _context.Vouchers
                .Where(v =>
                    v.CrID == tenantId &&
                    v.unitID == unitId &&
                    v.VoucherStatus == "PAID" &&
                    !v.Cancelled)
                .OrderByDescending(v => v.VoucherDate)
                .Take(2)
                .Select(v => new PaidPaymentDto
                {
                    VoucherId = v.VoucherID,
                    VoucherDate = v.VoucherDate,
                    Amount = v.Amount
                })
                .ToListAsync();

    
            return new TenantPaymentSummaryDto
            {
                PreviousUnpaidPayments = previousUnpaidPayments,
                NextUpcomingPayment = nextUpcoming,
                PreviousPaidPayments = paidPayments
            };
        }



    }
}
