using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.ManageMaintenance
{
    public interface IManageMaintenanceRepository
    {
        Task<PagedResultDto<object>> GetPendingList(
            int companyId, string? search = null,
            DateTime? date = null,
            int pageNumber = 1, int pageSize = 10);

        Task<PagedResultDto<object>> GetInProgressList(
            int companyId, string? search = null,
            DateTime? date = null,
            int pageNumber = 1, int pageSize = 10);

        Task<PagedResultDto<object>> GetCompletedList(
            int companyId, string? search = null,
            DateTime? date = null,
            int pageNumber = 1, int pageSize = 10);

        Task<XRS_ManageMaintenance> CreateMaintenance(ManageMaintenanceDto dto);

        Task<bool> UpdateMaintenanceStatus(int id, UpdateMaintenanceStatusDto dto);

        Task<int?> GetMaintenanceCompanyIdAsync(int id);
    }
}