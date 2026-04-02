using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.ManageMaintenance
{
    public class ManageMaintenanceRepository : IManageMaintenanceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ManageMaintenanceRepository(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ─────────────────────────────────────────
        // Pending List
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetPendingList(
            int companyId, string? search = null,
            DateTime? date = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "Pending"
                    && m.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(m =>
                    m.CreatedAt.Date == date.Value.Date);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.CompanyId,
                    m.ComplaintNo,
                    m.Unit,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Status,
                    m.AssignedEmployeeId,
                    m.CreatedAt,
                    PropertyName = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault(),
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    //Photos = _context.MaintenancePhotos
                    //    .Where(p => p.MaintenanceId == m.MaintenanceId)
                    //    .Select(p => p.PhotoUrl)
                    //    .ToList()
                    Photos = _context.MaintenancePhotos
                   .Where(p => p.MaintenanceId == m.MaintenanceId)
                   .Select(p => "http://localhost:5204" + p.PhotoUrl)
                  .ToList()
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

        // ─────────────────────────────────────────
        // In Progress List
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetInProgressList(
            int companyId, string? search = null,
            DateTime? date = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "InProgress"
                    && m.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(m =>
                    m.CreatedAt.Date == date.Value.Date);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.CompanyId,
                    m.ComplaintNo,
                    m.Unit,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Status,
                    m.AssignedEmployeeId,
                    m.CreatedAt,
                    PropertyName = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault(),
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    Photos = _context.MaintenancePhotos
                        .Where(p => p.MaintenanceId == m.MaintenanceId)
                        .Select(p => p.PhotoUrl)
                        .ToList()
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

        // ─────────────────────────────────────────
        // Completed List
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetCompletedList(
            int companyId, string? search = null,
            DateTime? date = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "Completed"
                    && m.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(m =>
                    m.CreatedAt.Date == date.Value.Date);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.CompanyId,
                    m.ComplaintNo,
                    m.Unit,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Status,
                    m.AssignedEmployeeId,
                    m.CreatedAt,
                    PropertyName = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault(),
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    Photos = _context.MaintenancePhotos
                        .Where(p => p.MaintenanceId == m.MaintenanceId)
                        .Select(p => p.PhotoUrl)
                        .ToList()
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

        // ─────────────────────────────────────────
        // Create New Request + Photos
        // ─────────────────────────────────────────
        public async Task<XRS_ManageMaintenance> CreateMaintenance(ManageMaintenanceDto dto)
        {
            var lastComplaint = await _context.ManageMaintenance
                .OrderByDescending(m => m.MaintenanceId)
                .FirstOrDefaultAsync();

            int nextNo = (lastComplaint == null) ? 500 :
                int.TryParse(lastComplaint.ComplaintNo.Replace("CMP", ""), out int last)
                ? last + 1 : 500;

            var maintenance = new XRS_ManageMaintenance
            {
                CompanyId = dto.CompanyId,
                ComplaintNo = $"CMP{nextNo}",
                PropertyId = dto.PropertyId,
                Unit = dto.Unit,
                CategoryId = dto.CategoryId,
                Complaint = dto.Complaint,
                PreferredVisitTime = dto.PreferredVisitTime,
                Status = "Pending",
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _context.ManageMaintenance.AddAsync(maintenance);
            await _context.SaveChangesAsync();

            // Save Photos
            if (dto.Photos != null && dto.Photos.Any())
            {
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath, "uploads", "maintenance");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var photo in dto.Photos)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    var maintenancePhoto = new XRS_MaintenancePhotos
                    {
                        MaintenanceId = maintenance.MaintenanceId,
                        PhotoUrl = $"/uploads/maintenance/{fileName}",
                        CreatedAt = DateTime.Now
                    };

                    await _context.MaintenancePhotos.AddAsync(maintenancePhoto);
                }

                await _context.SaveChangesAsync();
            }

            return maintenance;
        }

        // ─────────────────────────────────────────
        // Update Status
        // ─────────────────────────────────────────
        public async Task<bool> UpdateMaintenanceStatus(int id, UpdateMaintenanceStatusDto dto)
        {
            var maintenance = await _context.ManageMaintenance
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);

            if (maintenance == null) return false;

            maintenance.Status = dto.Status;
            maintenance.UpdatedAt = DateTime.Now;

            if (dto.AssignedEmployeeId.HasValue)
                maintenance.AssignedEmployeeId = dto.AssignedEmployeeId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int?> GetMaintenanceCompanyIdAsync(int id)
        {
            var maintenance = await _context.ManageMaintenance
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);
            
            return maintenance?.CompanyId;
        }
    }
}