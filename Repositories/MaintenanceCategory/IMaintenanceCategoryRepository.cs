using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.MaintenanceCategory
{
    public interface IMaintenanceCategoryRepository
    {
        Task<PagedResultDto<XRS_MaintenanceCategory>> GetMaintenanceCategoryByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<XRS_MaintenanceCategory>> GetMaintenanceCategories(int companyId);
        Task<IEnumerable<XRS_MaintenanceCategory>> GetMaintenanceCategoryById(int categoryId);
        Task<XRS_MaintenanceCategory> CreateMaintenanceCategory(MaintenanceCategoryDto dtoCategory);
        Task<bool> UpdateMaintenanceCategory(int id, MaintenanceCategoryDto category);
        Task<bool> DeleteMaintenanceCategory(int id);
    }
}
