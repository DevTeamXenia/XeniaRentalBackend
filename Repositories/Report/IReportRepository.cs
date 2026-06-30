
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Dtos.Report;
using XeniaRentalBackend.Dtos.Reports;


namespace XeniaRentalBackend.Repositories.Report
{
    public interface IReportRepository
    {
        Task<List<TenantOccupancyReportDto>> GetTenantOccupancyReportAsync( int companyId, int userId, int? propertyId, int? unitId, int? bedSpaceId, bool isBedSpace, string? search);
        public  Task<BalanceSheetResponseDto> GetIncomeExpenseAsync( int companyId,  DateTime? startDate, DateTime? endDate, int? propertyId);
        Task<List<MaintenanceReportDto>> GetMaintenanceReport(int companyId, int? tenantId, string? status, DateTime? fromDate, DateTime? toDate, string? zone, string? search);

    }
}
