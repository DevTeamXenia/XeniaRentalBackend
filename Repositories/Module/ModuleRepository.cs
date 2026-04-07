using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Module
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly ApplicationDbContext _context;

        public ModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────
        // Module CRUD (Based on SubTemple Logic)
        // ─────────────────────────────────────────

        public async Task<bool> CreateModuleAsync(int userId, ModuleDto dto)
        {
            try
            {
                var module = new XRS_Module
                {
                    ModuleName = dto.ModuleName,
                    ModuleDescription = dto.ModuleDescription,
                    ModuleActive = dto.ModuleActive
                };

                await _context.Module.AddAsync(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateModuleAsync(int id, ModuleDto dto, int userId)
        {
            var module = await _context.Module.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null) return false;

            module.ModuleName = dto.ModuleName;
            module.ModuleDescription = dto.ModuleDescription;
            module.ModuleActive = dto.ModuleActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResultDto<object>> GetModulesAsync(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Module.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.ModuleName.Contains(search));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.ModuleName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.ModuleId,
                    m.ModuleName,
                    m.ModuleDescription,
                    m.ModuleActive
                })
                .ToListAsync();

            return new PagedResultDto<object>
            {
                Data = items.Cast<object>().ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<XRS_Module?> GetModuleByIdAsync(int id)
        {
            return await _context.Module.FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<bool> DeleteModuleAsync(int id)
        {
            var module = await _context.Module.FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null) return false;

            _context.Module.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─────────────────────────────────────────
        // Module Mapping Logic (The Mapping System)
        // ─────────────────────────────────────────

        public async Task<XRS_PlanModuleMap> CreateOrUpdatePlanModuleAsync(XRS_PlanModuleMap dto)
        {
            // Senior's Logic: Check for existing mapping and update or create
            var existingMapping = await _context.PlanModuleMap
                .FirstOrDefaultAsync(x =>
                    x.PlanId == dto.PlanId &&
                    x.ModuleId == dto.ModuleId);

            if (existingMapping != null)
            {
                existingMapping.Active = dto.Active;

                _context.PlanModuleMap.Update(existingMapping);
                await _context.SaveChangesAsync();

                return existingMapping;
            }
            else
            {
                var newMapping = new XRS_PlanModuleMap
                {
                    PlanId = dto.PlanId,
                    ModuleId = dto.ModuleId,
                    Active = dto.Active
                };

                await _context.PlanModuleMap.AddAsync(newMapping);
                await _context.SaveChangesAsync();

                return newMapping;
            }
        }

        public async Task<List<XRS_PlanModuleMap>> GetPlanModuleMappingsAsync(int planId)
        {
            return await _context.PlanModuleMap
                .Where(m => m.PlanId == planId)
                .OrderBy(m => m.ModuleId)
                .ToListAsync();
        }

        public async Task<List<XRS_Module>> GetSyncModulesAsync()
        {
            return await _context.Module
                .Where(m => m.ModuleActive)
                .OrderBy(m => m.ModuleName)
                .ToListAsync();
        }
    }
}
