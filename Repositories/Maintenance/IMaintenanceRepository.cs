using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.ManageMaintenance
{
    public interface IMaintenanceRepository
    {

        Task<List<XRS_Maintenance>> GetMaintenance(int companyId, int? tenantId);
        Task<XRS_Maintenance> CreateMaintenance(MaintenanceDto dto);
        Task<bool> UpdateMaintenance(int maintainceId, int? employeeId, string status);

 
        Task<PagedResultDto<XRS_MaintenanceCategory>> GetMaintenanceCategoryByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<XRS_MaintenanceCategory>> GetMaintenanceCategories(int companyId);
        Task<IEnumerable<XRS_MaintenanceCategory>> GetMaintenanceCategoryById(int categoryId);
        Task<XRS_MaintenanceCategory> CreateMaintenanceCategory(MaintenanceCategoryDto dtoCategory);
        Task<bool> UpdateMaintenanceCategory(int id, MaintenanceCategoryDto category);
        Task<bool> DeleteMaintenanceCategory(int id);
    }
}
