using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaTenoraBackend.Dtos;

namespace XeniaRentalBackend.Repositories.ManageMaintenance
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly ApplicationDbContext _context;
      
        public MaintenanceRepository(ApplicationDbContext context)
        {
            _context = context;
          
        }

        public async Task<List<MaintenanceResponseDto>> GetMaintenance(int companyId, int? tenantId, string? search, string? status = null)
        {
            var query = from m in _context.ManageMaintenance
                        join p in _context.Properties on m.PropertyId equals p.PropID into pp
                        from property in pp.DefaultIfEmpty()
                        join u in _context.Units on m.UnitId equals u.UnitId into uu
                        from unit in uu.DefaultIfEmpty()
                        join t in _context.Tenants on m.TenantId equals t.tenantID into tt
                        from tenant in tt.DefaultIfEmpty()
                        join c in _context.MaintenanceCategories on m.CategoryId equals c.CategoryId into cc
                        from category in cc.DefaultIfEmpty()
                        where m.CompanyId == companyId && m.IsActive
                        select new MaintenanceResponseDto
                        {
                            MaintenanceId = m.MaintenanceId,
                            CompanyId = m.CompanyId,
                            TenantId = m.TenantId,
                            TenantName = tenant != null ? tenant.tenantName : null,
                            ComplaintNo = m.ComplaintNo,
                            PropertyId = m.PropertyId,
                            PropertyName = property != null ? property.propertyName : null,
                            UnitId = m.UnitId,
                            UnitName = unit != null ? unit.UnitName : null,
                            CategoryId = m.CategoryId,
                            CategoryName = category != null ? category.CategoryName : null,
                            Complaint = m.Complaint,
                            PreferredVisitTime = m.PreferredVisitTime,
                            Status = m.Status,
                            AssignedEmployeeId = m.AssignedEmployeeId,
                            IsActive = m.IsActive,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt,
                            Photos = m.Photos.ToList()
                        };

            if (tenantId.HasValue)
            {
                query = query.Where(m => m.TenantId == tenantId.Value);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status); 
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.PropertyName.Contains(search) ||
                    m.UnitName.Contains(search) ||
                    m.CategoryName.Contains(search) ||
                    m.Complaint.Contains(search) ||
                    m.Status.Contains(search)
                );
            }

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<MaintenanceDetailsDto> GetMaintenanceDetails(int maintenanceId, int companyId)
        {
            var baseQuery = from m in _context.ManageMaintenance
                            join p in _context.Properties on m.PropertyId equals p.PropID into pp
                            from property in pp.DefaultIfEmpty()
                            join u in _context.Units on m.UnitId equals u.UnitId into uu
                            from unit in uu.DefaultIfEmpty()
                            join t in _context.Tenants on m.TenantId equals t.tenantID into tt
                            from tenant in tt.DefaultIfEmpty()
                            join c in _context.MaintenanceCategories on m.CategoryId equals c.CategoryId into cc
                            from category in cc.DefaultIfEmpty()
                            where m.CompanyId == companyId && m.IsActive
                            select new MaintenanceResponseDto
                            {
                                MaintenanceId = m.MaintenanceId,
                                CompanyId = m.CompanyId,
                                TenantId = m.TenantId,
                                TenantName = tenant != null ? tenant.tenantName : null,
                                ComplaintNo = m.ComplaintNo,
                                PropertyId = m.PropertyId,
                                PropertyName = property != null ? property.propertyName : null,
                                UnitId = m.UnitId,
                                UnitName = unit != null ? unit.UnitName : null,
                                CategoryId = m.CategoryId,
                                CategoryName = category != null ? category.CategoryName : null,
                                Complaint = m.Complaint,
                                PreferredVisitTime = m.PreferredVisitTime,
                                Status = m.Status,
                                AssignedEmployeeId = m.AssignedEmployeeId,
                                IsActive = m.IsActive,
                                CreatedAt = m.CreatedAt,
                                UpdatedAt = m.UpdatedAt,
                                Photos = m.Photos.ToList()
                            };

            var current = await baseQuery
                .FirstOrDefaultAsync(x => x.MaintenanceId == maintenanceId);

            if (current == null)
                return null;

        
            var history = await baseQuery
                .Where(x => x.UnitId == current.UnitId && x.MaintenanceId != maintenanceId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return new MaintenanceDetailsDto
            {
                Current = current,
                History = history
            };
        }

        public async Task<MaintenanceResponseDto> CreateMaintenance(MaintenanceDto dto)
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

            var property = await _context.Properties.FindAsync(dto.PropertyId);
            var unit = await _context.Units.FindAsync(dto.UnitId);
            var tenant = await _context.Tenants.FindAsync(dto.TenantId);
            var category = await _context.MaintenanceCategories.FindAsync(dto.CategoryId);

            return new MaintenanceResponseDto
            {
                MaintenanceId = maintenance.MaintenanceId,
                CompanyId = maintenance.CompanyId,
                TenantId = maintenance.TenantId,
                TenantName = tenant?.tenantName,
                ComplaintNo = maintenance.ComplaintNo,
                PropertyId = maintenance.PropertyId,
                PropertyName = property?.propertyName,
                UnitId = maintenance.UnitId,
                UnitName = unit?.UnitName,
                CategoryId = maintenance.CategoryId,
                CategoryName = category?.CategoryName,
                Complaint = maintenance.Complaint,
                PreferredVisitTime = maintenance.PreferredVisitTime,
                Status = maintenance.Status,
                AssignedEmployeeId = maintenance.AssignedEmployeeId,
                IsActive = maintenance.IsActive,
                CreatedAt = maintenance.CreatedAt,
                UpdatedAt = maintenance.UpdatedAt,
                Photos = await _context.MaintenancePhotos.Where(p => p.MaintenanceId == maintenance.MaintenanceId).ToListAsync()
            };
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

        public async Task<List<MaintenanceReportDto>> GetMaintenanceReport(int companyId, int? tenantId, string? status, DateTime? fromDate, DateTime? toDate, string? zone, string? search)
        {
            var query = from m in _context.ManageMaintenance
                        join p in _context.Properties on m.PropertyId equals p.PropID into pp
                        from property in pp.DefaultIfEmpty()
                        join u in _context.Units on m.UnitId equals u.UnitId into uu
                        from unit in uu.DefaultIfEmpty()
                        join t in _context.Tenants on m.TenantId equals t.tenantID into tt
                        from tenant in tt.DefaultIfEmpty()
                        join c in _context.MaintenanceCategories on m.CategoryId equals c.CategoryId into cc
                        from category in cc.DefaultIfEmpty()
                        join e in _context.Employee on m.AssignedEmployeeId equals e.EmployeeId into ee
                        from employee in ee.DefaultIfEmpty()
                        where m.CompanyId == companyId && m.IsActive
                        select new { m, property, unit, tenant, category, employee };

            // Filters
            if (tenantId.HasValue && tenantId > 0)
            {
                query = query.Where(x => x.m.TenantId == tenantId);
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(x => x.m.Status == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.m.CreatedAt.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.m.CreatedAt.Date <= toDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(zone) && zone != "All")
            {
                query = query.Where(x => x.employee != null && x.employee.AreaZone.Contains(zone));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.m.ComplaintNo.Contains(search) ||
                                         x.m.Complaint.Contains(search) ||
                                         (x.tenant != null && x.tenant.tenantName.Contains(search)) ||
                                         (x.employee != null && x.employee.Name.Contains(search)));
            }

            var result = await query.Select(x => new MaintenanceReportDto
            {
                MaintenanceId = x.m.MaintenanceId,
                ComplaintNo = x.m.ComplaintNo,
                CreatedAt = x.m.CreatedAt,
                PropertyUnit = (x.property != null ? x.property.propertyName : "") + (x.unit != null ? " - " + x.unit.UnitName : ""),
                RegisteredBy = x.tenant != null ? x.tenant.tenantName : "Owner",
                CategoryName = x.category != null ? x.category.CategoryName : "",
                Complaint = x.m.Complaint,
                Status = x.m.Status,
                EngineerName = x.employee != null ? x.employee.Name : "Unassigned",
                Zone = x.employee != null ? x.employee.AreaZone : "",
                UpdatedAt = x.m.UpdatedAt
            }).OrderByDescending(x => x.CreatedAt).ToListAsync();

            return result;
        }
    }
}
