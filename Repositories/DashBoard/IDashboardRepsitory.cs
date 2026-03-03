using XeniaRentalBackend.Controllers;
using XeniaRentalBackend.Dtos;


namespace XeniaRentalBackend.Repositories.Dashboard
{
    public interface IDashboardRepsitory
    {
        Task<RentDashboardDto> GetRentDashboardAsync(int companyid, DateTime fromDate, DateTime toDate);
        Task<MonthlyRevenueResponseDto> GetMonthlyRentRevenueAsync(int companyid, int year);
        Task<TenantPaymentBannerDto> GetTenantPaymentBannerAsync(int tenantId, int companyId);
        Task<TenantPaymentSummaryDto> GetTenantPaymentsAsync(int unitId);
    }
}
