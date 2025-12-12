using XeniaRentalApi.Dtos;
using XeniaRentalApi.DTOs;

namespace XeniaRentalApi.Repositories.Dashboard
{
    public interface IDashboardRepsitory
    {
        Task<RentDashboardDto> GetRentDashboardAsync(int companyid, DateTime fromDate, DateTime toDate);
        Task<List<MonthlyRevenueDto>> GetMonthlyRentRevenueAsync(int companyid, int year);
        Task<TenantPaymentBannerDto> GetTenantPaymentBannerAsync(int tenantId, int companyId);
    }
}
