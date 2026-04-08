using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.ManageMaintenance
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly ApplicationDbContext _context;
      
        public MaintenanceRepository(ApplicationDbContext context)
        {
            _context = context;
          
        }

        public async Task<List<XRS_Maintenance>> GetMaintenance(int companyId, int? tenantId, string? status)
        {
            var query = _context.ManageMaintenance
                .Include(m => m.Photos)
                .Where(m => m.CompanyId == companyId && m.IsActive);


            if (tenantId.HasValue)
            {
                query = query.Where(m => m.TenantId == tenantId.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status); // ✅ FILTER
            }

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }


        public async Task<XRS_Maintenance> CreateMaintenance(MaintenanceDto dto)
        {
            var lastComplaint = await _context.ManageMaintenance
                .OrderByDescending(m => m.MaintenanceId)
                .FirstOrDefaultAsync();

            int nextNo = 500;

            if (lastComplaint != null && !string.IsNullOrEmpty(lastComplaint.ComplaintNo))
            {
                var numberPart = lastComplaint.ComplaintNo.Replace("CMP", "");
                if (int.TryParse(numberPart, out int last))
                {
                    nextNo = last + 1;
                }
            }

            var maintenance = new XRS_Maintenance
            {
                CompanyId = dto.CompanyId,
                TenantId = dto.TenantId,
                ComplaintNo = $"CMP{nextNo}",
                PropertyId = dto.PropertyId,
                UnitId = dto.UnitId,
                CategoryId = dto.CategoryId,
                Complaint = dto.Complaint,
                PreferredVisitTime = dto.PreferredVisitTime,
                Status = "Pending",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.ManageMaintenance.AddAsync(maintenance);
            await _context.SaveChangesAsync();

          
            if (dto.Photos != null && dto.Photos.Any())
            {
                foreach (var photo in dto.Photos)
                {
                    var maintenancePhoto = new XRS_MaintenancePhotos
                    {
                        MaintenanceId = maintenance.MaintenanceId,
                        PhotoUrl = photo.PhotoUrl, 
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.MaintenancePhotos.AddAsync(maintenancePhoto);
                }

                await _context.SaveChangesAsync();
            }

            return maintenance;
        }

        public async Task<bool> UpdateMaintenance(int maintainceId, int? employeeId, string status)
        {
            var maintenance = await _context.ManageMaintenance
                .FirstOrDefaultAsync(m => m.MaintenanceId == maintainceId);

            if (maintenance == null) return false;

            maintenance.Status = status;
            maintenance.UpdatedAt = DateTime.Now;

            if (employeeId != null)
                maintenance.AssignedEmployeeId = employeeId;

            await _context.SaveChangesAsync();
            return true;
        }

      

        public async Task<IEnumerable<XRS_MaintenanceCategory>> GetMaintenanceCategories(int companyId)
        {
            return await _context.MaintenanceCategories
                .Where(u => u.CompanyId == companyId && u.IsActive == true)
                .Select(u => new XRS_MaintenanceCategory
                {
                    CategoryId = u.CategoryId,
                    CompanyId = u.CompanyId,
                    CategoryName = u.CategoryName,
                    SLADays = u.SLADays,
                    SLAHours = u.SLAHours,
                    IsActive = u.IsActive
                }).ToListAsync();
        }

        public async Task<PagedResultDto<XRS_MaintenanceCategory>> GetMaintenanceCategoryByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.MaintenanceCategories.AsQueryable();
            query = query.Where(u => u.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.CategoryName.Contains(search));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.CategoryName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new XRS_MaintenanceCategory
                {
                    CategoryId = u.CategoryId,
                    CompanyId = u.CompanyId,
                    CategoryName = u.CategoryName,
                    SLADays = u.SLADays,
                    SLAHours = u.SLAHours,
                    IsActive = u.IsActive
                }).ToListAsync();

            return new PagedResultDto<XRS_MaintenanceCategory>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<IEnumerable<XRS_MaintenanceCategory>> GetMaintenanceCategoryById(int categoryId)
        {
            return await _context.MaintenanceCategories
                .Where(u => u.CategoryId == categoryId)
                .Select(u => new XRS_MaintenanceCategory
                {
                    CategoryId = u.CategoryId,
                    CompanyId = u.CompanyId,
                    CategoryName = u.CategoryName,
                    SLADays = u.SLADays,
                    SLAHours = u.SLAHours,
                    IsActive = u.IsActive
                }).ToListAsync();
        }

        public async Task<XRS_MaintenanceCategory> CreateMaintenanceCategory(MaintenanceCategoryDto dtoCategory)
        {
            var category = new XRS_MaintenanceCategory
            {
                CategoryName = dtoCategory.CategoryName,
                CompanyId = dtoCategory.CompanyId,
                SLADays = dtoCategory.SLADays,
                SLAHours = dtoCategory.SLAHours,
                IsActive = dtoCategory.IsActive
            };
            await _context.MaintenanceCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateMaintenanceCategory(int id, MaintenanceCategoryDto category)
        {
            var updateCategory = await _context.MaintenanceCategories.FirstOrDefaultAsync(u => u.CategoryId == id);
            if (updateCategory == null) return false;

            updateCategory.CategoryName = category.CategoryName;
            updateCategory.CompanyId = category.CompanyId;
            updateCategory.SLADays = category.SLADays;
            updateCategory.SLAHours = category.SLAHours;
            updateCategory.IsActive = category.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMaintenanceCategory(int id)
        {
            var category = await _context.MaintenanceCategories.FirstOrDefaultAsync(u => u.CategoryId == id);
            if (category == null) return false;

            category.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
