using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Module
{
    public interface IModuleRepository
    {
        // CRUD for Modules (Like SubTemple in Example)
        Task<bool> CreateModuleAsync(int userId, ModuleDto dto);
        Task<bool> UpdateModuleAsync(int id, ModuleDto dto, int userId);
        Task<PagedResultDto<object>> GetModulesAsync(string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<XRS_Module?> GetModuleByIdAsync(int id);
        Task<bool> DeleteModuleAsync(int id);

        // Module-wise Plan Mapping (The "Senior Logic" Map)
        Task<XRS_PlanModuleMap> CreateOrUpdatePlanModuleAsync(XRS_PlanModuleMap dto);
        Task<List<XRS_PlanModuleMap>> GetPlanModuleMappingsAsync(int planId);
        Task<List<XRS_Module>> GetSyncModulesAsync();
    }
}
