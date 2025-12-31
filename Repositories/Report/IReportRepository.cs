
using XeniaRentalBackend.Dtos.Reports;


namespace XeniaRentalBackend.Repositories.Report
{
    public interface IReportRepository
    {
        Task<List<TenantOccupancyReportDto>> GetTenantOccupancyReportAsync(
                    int companyId,
                    int? propertyId,
                    int? unitId,
                    int? bedSpaceId,
                    bool isBedSpace,
                    string? search
                );

    }
}
