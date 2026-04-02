//using Microsoft.EntityFrameworkCore;
//using XeniaRentalBackend.Dtos;
//using XeniaRentalBackend.Models;

//namespace XeniaRentalBackend.Repositories.AdminComplaint
//{
//    public class AdminComplaintRepository : IAdminComplaintRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public AdminComplaintRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ─────────────────────────────────────────
//        // New Complaints — Status = Pending
//        // ─────────────────────────────────────────
//        public async Task<PagedResultDto<object>> GetNewComplaints(
//            int companyId, string? search = null,
//            string? zone = null,
//            int pageNumber = 1, int pageSize = 10)
//        {
//            var query = _context.ManageMaintenance
//                .Where(m => m.CompanyId == companyId
//                    && m.Status == "Pending"
//                    && m.IsActive == true)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(search))
//                query = query.Where(m =>
//                    m.ComplaintNo.Contains(search) ||
//                    m.Unit.Contains(search) ||
//                    m.Complaint.Contains(search));

//            var totalRecords = await query.CountAsync();

//            var items = await query
//                .OrderByDescending(m => m.CreatedAt)
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .Select(m => new
//                {
//                    m.MaintenanceId,
//                    m.ComplaintNo,
//                    m.CreatedAt,
//                    m.Status,
//                    m.Complaint,
//                    m.PreferredVisitTime,
//                    m.Unit,

//                    // Property – Unit
//                    PropertyUnit = _context.Properties
//                        .Where(p => p.PropID == m.PropertyId)
//                        .Select(p => p.propertyName)
//                        .FirstOrDefault() + " – " + m.Unit,

//                    // Category Name
//                    CategoryName = _context.MaintenanceCategories
//                        .Where(c => c.CategoryId == m.CategoryId)
//                        .Select(c => c.CategoryName)
//                        .FirstOrDefault(),

//                    // Registered By — Tenant Name
//                    RegisteredBy = _context.Tenants
//                        .Where(t => t.tenantID == m.TenantId)
//                        .Select(t => t.tenantName)
//                        .FirstOrDefault(),

//                    // Engineer Name
//                    EngineerName = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.Name)
//                        .FirstOrDefault(),

//                    // Zone
//                    Zone = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.AreaZone)
//                        .FirstOrDefault(),
//                })
//                .ToListAsync();

//            // Zone Filter
//            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
//                items = items.Where(i => i.Zone == zone).ToList();

//            return new PagedResultDto<object>
//            {
//                Data = items.Cast<object>().ToList(),
//                PageNumber = pageNumber,
//                PageSize = pageSize,
//                TotalRecords = totalRecords
//            };
//        }

//        // ─────────────────────────────────────────
//        // In Progress — Status = InProgress
//        // ─────────────────────────────────────────
//        public async Task<PagedResultDto<object>> GetInProgressComplaints(
//            int companyId, string? search = null,
//            string? zone = null,
//            int pageNumber = 1, int pageSize = 10)
//        {
//            var query = _context.ManageMaintenance
//                .Where(m => m.CompanyId == companyId
//                    && m.Status == "InProgress"
//                    && m.IsActive == true)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(search))
//                query = query.Where(m =>
//                    m.ComplaintNo.Contains(search) ||
//                    m.Unit.Contains(search) ||
//                    m.Complaint.Contains(search));

//            var totalRecords = await query.CountAsync();

//            var items = await query
//                .OrderByDescending(m => m.CreatedAt)
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .Select(m => new
//                {
//                    m.MaintenanceId,
//                    m.ComplaintNo,
//                    m.CreatedAt,
//                    m.Status,
//                    m.Complaint,
//                    m.PreferredVisitTime,
//                    m.Unit,
//                    PropertyUnit = _context.Properties
//                        .Where(p => p.PropID == m.PropertyId)
//                        .Select(p => p.propertyName)
//                        .FirstOrDefault() + " – " + m.Unit,
//                    CategoryName = _context.MaintenanceCategories
//                        .Where(c => c.CategoryId == m.CategoryId)
//                        .Select(c => c.CategoryName)
//                        .FirstOrDefault(),
//                    RegisteredBy = _context.Tenants
//                        .Where(t => t.tenantID == m.TenantId)
//                        .Select(t => t.tenantName)
//                        .FirstOrDefault(),
//                    EngineerName = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.Name)
//                        .FirstOrDefault(),
//                    Zone = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.AreaZone)
//                        .FirstOrDefault(),
//                })
//                .ToListAsync();

//            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
//                items = items.Where(i => i.Zone == zone).ToList();

//            return new PagedResultDto<object>
//            {
//                Data = items.Cast<object>().ToList(),
//                PageNumber = pageNumber,
//                PageSize = pageSize,
//                TotalRecords = totalRecords
//            };
//        }

//        // ─────────────────────────────────────────
//        // Closed — Status = Completed
//        // ─────────────────────────────────────────
//        public async Task<PagedResultDto<object>> GetClosedComplaints(
//            int companyId, string? search = null,
//            string? zone = null,
//            int pageNumber = 1, int pageSize = 10)
//        {
//            var query = _context.ManageMaintenance
//                .Where(m => m.CompanyId == companyId
//                    && m.Status == "Completed"
//                    && m.IsActive == true)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(search))
//                query = query.Where(m =>
//                    m.ComplaintNo.Contains(search) ||
//                    m.Unit.Contains(search) ||
//                    m.Complaint.Contains(search));

//            var totalRecords = await query.CountAsync();

//            var items = await query
//                .OrderByDescending(m => m.CreatedAt)
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .Select(m => new
//                {
//                    m.MaintenanceId,
//                    m.ComplaintNo,
//                    m.CreatedAt,
//                    m.Status,
//                    m.Complaint,
//                    m.PreferredVisitTime,
//                    m.Unit,
//                    PropertyUnit = _context.Properties
//                        .Where(p => p.PropID == m.PropertyId)
//                        .Select(p => p.propertyName)
//                        .FirstOrDefault() + " – " + m.Unit,
//                    CategoryName = _context.MaintenanceCategories
//                        .Where(c => c.CategoryId == m.CategoryId)
//                        .Select(c => c.CategoryName)
//                        .FirstOrDefault(),
//                    RegisteredBy = _context.Tenants
//                        .Where(t => t.tenantID == m.TenantId)
//                        .Select(t => t.tenantName)
//                        .FirstOrDefault(),
//                    EngineerName = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.Name)
//                        .FirstOrDefault(),
//                    Zone = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.AreaZone)
//                        .FirstOrDefault(),
//                })
//                .ToListAsync();

//            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
//                items = items.Where(i => i.Zone == zone).ToList();

//            return new PagedResultDto<object>
//            {
//                Data = items.Cast<object>().ToList(),
//                PageNumber = pageNumber,
//                PageSize = pageSize,
//                TotalRecords = totalRecords
//            };
//        }

//        // ─────────────────────────────────────────
//        // Overdue — Pending + 24 hours കഴിഞ്ഞത്
//        // ─────────────────────────────────────────
//        public async Task<PagedResultDto<object>> GetOverdueComplaints(
//            int companyId, string? search = null,
//            string? zone = null,
//            int pageNumber = 1, int pageSize = 10)
//        {
//            var overdueTime = DateTime.Now.AddHours(-24);

//            var query = _context.ManageMaintenance
//                .Where(m => m.CompanyId == companyId
//                    && m.Status == "Pending"
//                    && m.IsActive == true
//                    && m.CreatedAt <= overdueTime)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(search))
//                query = query.Where(m =>
//                    m.ComplaintNo.Contains(search) ||
//                    m.Unit.Contains(search) ||
//                    m.Complaint.Contains(search));

//            var totalRecords = await query.CountAsync();

//            var items = await query
//                .OrderByDescending(m => m.CreatedAt)
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .Select(m => new
//                {
//                    m.MaintenanceId,
//                    m.ComplaintNo,
//                    m.CreatedAt,
//                    m.Status,
//                    m.Complaint,
//                    m.PreferredVisitTime,
//                    m.Unit,
//                    PropertyUnit = _context.Properties
//                        .Where(p => p.PropID == m.PropertyId)
//                        .Select(p => p.propertyName)
//                        .FirstOrDefault() + " – " + m.Unit,
//                    CategoryName = _context.MaintenanceCategories
//                        .Where(c => c.CategoryId == m.CategoryId)
//                        .Select(c => c.CategoryName)
//                        .FirstOrDefault(),
//                    RegisteredBy = _context.Tenants
//                        .Where(t => t.tenantID == m.TenantId)
//                        .Select(t => t.tenantName)
//                        .FirstOrDefault(),
//                    EngineerName = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.Name)
//                        .FirstOrDefault(),
//                    Zone = _context.Employee
//                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
//                        .Select(e => e.AreaZone)
//                        .FirstOrDefault(),
//                })
//                .ToListAsync();

//            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
//                items = items.Where(i => i.Zone == zone).ToList();

//            return new PagedResultDto<object>
//            {
//                Data = items.Cast<object>().ToList(),
//                PageNumber = pageNumber,
//                PageSize = pageSize,
//                TotalRecords = totalRecords
//            };
//        }

//        // ─────────────────────────────────────────
//        // Assign — Modal Save
//        // ─────────────────────────────────────────
//        public async Task<XRS_ComplaintAssignment> AssignComplaint(ComplaintAssignmentDto dto)
//        {
//            // Maintenance Details എടുക്കുക
//            var maintenance = await _context.ManageMaintenance
//                .FirstOrDefaultAsync(m => m.MaintenanceId == dto.MaintenanceId);

//            if (maintenance == null)
//                throw new Exception("Maintenance not found.");

//            // Employee Zone എടുക്കുക
//            var employee = await _context.Employee
//                .FirstOrDefaultAsync(e => e.EmployeeId == dto.AssignedEmployeeId);

//            // Assignment Create ചെയ്യുക
//            var assignment = new XRS_ComplaintAssignment
//            {
//                CompanyId = dto.CompanyId,
//                MaintenanceId = dto.MaintenanceId,
//                ComplaintNo = maintenance.ComplaintNo,
//                PropertyId = maintenance.PropertyId,
//                Unit = maintenance.Unit,
//                TenantId = maintenance.TenantId,
//                CategoryId = dto.CategoryId,
//                UpdatedCategoryId = dto.UpdatedCategoryId,
//                AssignedEmployeeId = dto.AssignedEmployeeId,
//                Instructions = dto.Instructions,
//                Complaint = maintenance.Complaint,
//                PreferredVisitTime = maintenance.PreferredVisitTime,
//                Status = "InProgress",
//                Zone = employee?.AreaZone,
//                IsActive = true,
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };

//            await _context.ComplaintAssignments.AddAsync(assignment);

//            // Maintenance Status Update ചെയ്യുക
//            maintenance.Status = "InProgress";
//            maintenance.AssignedEmployeeId = dto.AssignedEmployeeId;
//            maintenance.UpdatedAt = DateTime.Now;

//            await _context.SaveChangesAsync();
//            return assignment;
//        }
//    }
//}






using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Dtos.XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.AdminComplaint
{
    public class AdminComplaintRepository : IAdminComplaintRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminComplaintRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────
        // New Complaints
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetNewComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "Pending"
                    && m.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.ComplaintNo,
                    m.CreatedAt,
                    m.Status,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Unit,
                    PropertyUnit = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault() + " – " + m.Unit,
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    RegisteredBy = _context.Tenants
                        .Where(t => t.tenantID == m.TenantId)
                        .Select(t => t.tenantName)
                        .FirstOrDefault(),
                    EngineerName = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.Name)
                        .FirstOrDefault(),
                    Zone = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.AreaZone)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
                items = items.Where(i => i.Zone == zone).ToList();

            return new PagedResultDto<object>
            {
                Data = items.Cast<object>().ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        // ─────────────────────────────────────────
        // In Progress
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetInProgressComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "InProgress"
                    && m.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.ComplaintNo,
                    m.CreatedAt,
                    m.Status,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Unit,
                    PropertyUnit = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault() + " – " + m.Unit,
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    RegisteredBy = _context.Tenants
                        .Where(t => t.tenantID == m.TenantId)
                        .Select(t => t.tenantName)
                        .FirstOrDefault(),
                    EngineerName = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.Name)
                        .FirstOrDefault(),
                    Zone = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.AreaZone)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
                items = items.Where(i => i.Zone == zone).ToList();

            return new PagedResultDto<object>
            {
                Data = items.Cast<object>().ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        // ─────────────────────────────────────────
        // Closed
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetClosedComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "Completed"
                    && m.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.ComplaintNo,
                    m.CreatedAt,
                    m.Status,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Unit,
                    PropertyUnit = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault() + " – " + m.Unit,
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    RegisteredBy = _context.Tenants
                        .Where(t => t.tenantID == m.TenantId)
                        .Select(t => t.tenantName)
                        .FirstOrDefault(),
                    EngineerName = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.Name)
                        .FirstOrDefault(),
                    Zone = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.AreaZone)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
                items = items.Where(i => i.Zone == zone).ToList();

            return new PagedResultDto<object>
            {
                Data = items.Cast<object>().ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        // ─────────────────────────────────────────
        // Overdue
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<object>> GetOverdueComplaints(
            int companyId, string? search = null,
            string? zone = null,
            int pageNumber = 1, int pageSize = 10)
        {
            var overdueTime = DateTime.Now.AddHours(-24);

            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId
                    && m.Status == "Pending"
                    && m.IsActive == true
                    && m.CreatedAt <= overdueTime)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.ComplaintNo,
                    m.CreatedAt,
                    m.Status,
                    m.Complaint,
                    m.PreferredVisitTime,
                    m.Unit,
                    PropertyUnit = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault() + " – " + m.Unit,
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    RegisteredBy = _context.Tenants
                        .Where(t => t.tenantID == m.TenantId)
                        .Select(t => t.tenantName)
                        .FirstOrDefault(),
                    EngineerName = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.Name)
                        .FirstOrDefault(),
                    Zone = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.AreaZone)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
                items = items.Where(i => i.Zone == zone).ToList();

            return new PagedResultDto<object>
            {
                Data = items.Cast<object>().ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        // ─────────────────────────────────────────
        // Assign Complaint
        // ─────────────────────────────────────────
        public async Task<XRS_ComplaintAssignment> AssignComplaint(ComplaintAssignmentDto dto)
        {
            var maintenance = await _context.ManageMaintenance
                .FirstOrDefaultAsync(m => m.MaintenanceId == dto.MaintenanceId);

            if (maintenance == null)
                throw new Exception("Maintenance not found.");

            var employee = await _context.Employee
                .FirstOrDefaultAsync(e => e.EmployeeId == dto.AssignedEmployeeId);

            var assignment = new XRS_ComplaintAssignment
            {
                CompanyId = dto.CompanyId,
                MaintenanceId = dto.MaintenanceId,
                ComplaintNo = maintenance.ComplaintNo,
                PropertyId = maintenance.PropertyId,
                Unit = maintenance.Unit,
                TenantId = maintenance.TenantId ?? 0,
                CategoryId = dto.CategoryId,
                UpdatedCategoryId = dto.UpdatedCategoryId,
                AssignedEmployeeId = dto.AssignedEmployeeId,
                Instructions = dto.Instructions,
                Complaint = maintenance.Complaint,
                PreferredVisitTime = maintenance.PreferredVisitTime,
                Status = "InProgress",
                Zone = employee?.AreaZone,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _context.ComplaintAssignments.AddAsync(assignment);


            var history = new XRS_ComplaintHistory
            {
                MaintenanceId = dto.MaintenanceId,
                CompanyId = dto.CompanyId,
                ReportDate = DateTime.Now,
                Report = $"Service/Complaint Assigned to {employee?.Name} ({employee?.Specialization}) by Admin",
                CreatedBy = "Admin"
            };
            await _context.ComplaintHistories.AddAsync(history);

            // Maintenance Status Update
            maintenance.Status = "InProgress";
            maintenance.AssignedEmployeeId = dto.AssignedEmployeeId;
            maintenance.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return assignment;
        }





        // ─────────────────────────────────────────
        // Complaint Details — Grid Click
        // ─────────────────────────────────────────
        public async Task<object?> GetComplaintDetails(int maintenanceId)
        {
            var maintenance = await _context.ManageMaintenance
                .FirstOrDefaultAsync(m => m.MaintenanceId == maintenanceId);

            if (maintenance == null) return null;

            // Property Name
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.PropID == maintenance.PropertyId);

            // Category Name
            var category = await _context.MaintenanceCategories
                .FirstOrDefaultAsync(c => c.CategoryId == maintenance.CategoryId);

            // Photos
            var photos = await _context.MaintenancePhotos
                .Where(p => p.MaintenanceId == maintenanceId)
                .Select(p => p.PhotoUrl)
                .ToListAsync();

            // History
            var history = await _context.ComplaintHistories
                .Where(h => h.MaintenanceId == maintenanceId)
                .OrderBy(h => h.ReportDate)
                .Select(h => new
                {
                    Date = h.ReportDate.ToString("dd/MM/yyyy"),
                    Time = h.ReportDate.ToString("hh:mm tt"),
                    h.Report,
                    h.CreatedBy
                })
                .ToListAsync();

            // Service Numbers
            var serviceNos = await _context.ComplaintServiceNo
                .Where(s => s.MaintenanceId == maintenanceId)
                .OrderByDescending(s => s.ServiceDate)
                .Select(s => new
                {
                    Date = s.ServiceDate.ToString("dd/MM/yyyy"),
                    s.ServiceNo
                })
                .ToListAsync();

            return new
            {
                maintenance.MaintenanceId,
                maintenance.ComplaintNo,
                maintenance.Status,
                maintenance.Complaint,
                maintenance.PreferredVisitTime,
                maintenance.Unit,
                PropertyName = property?.propertyName,
                CategoryName = category?.CategoryName,
                Photos = photos,
                History = history,
                ServiceNos = serviceNos
            };
        }
            

           public async Task<PagedResultDto<object>> GetMaintenanceReport(
    int companyId,
    string? search = null,
    string? status = null,
    string? zone = null,
    DateTime? dateFrom = null,
    DateTime? dateTo = null,
    int pageNumber = 1,
    int pageSize = 10)
        {
            var query = _context.ManageMaintenance
                .Where(m => m.CompanyId == companyId && m.IsActive == true)
                .AsQueryable();

            // Search Filter
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m =>
                    m.ComplaintNo.Contains(search) ||
                    m.Unit.Contains(search) ||
                    m.Complaint.Contains(search));

            // Status Filter
            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
                query = query.Where(m => m.Status == status);

            // Date From Filter
            if (dateFrom.HasValue)
                query = query.Where(m => m.CreatedAt.Date >= dateFrom.Value.Date);

            // Date To Filter
            if (dateTo.HasValue)
                query = query.Where(m => m.CreatedAt.Date <= dateTo.Value.Date);

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.MaintenanceId,
                    m.ComplaintNo,
                    m.CreatedAt,
                    m.UpdatedAt,
                    m.Status,
                    m.Complaint,
                    m.Unit,

                    // Property – Unit
                    PropertyUnit = _context.Properties
                        .Where(p => p.PropID == m.PropertyId)
                        .Select(p => p.propertyName)
                        .FirstOrDefault() + " – " + m.Unit,

                    // Category Name
                    CategoryName = _context.MaintenanceCategories
                        .Where(c => c.CategoryId == m.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),

                    // Registered By — Tenant
                    RegisteredBy = _context.Tenants
                        .Where(t => t.tenantID == m.TenantId)
                        .Select(t => t.tenantName)
                        .FirstOrDefault(),

                    // Engineer Name
                    EngineerName = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.Name)
                        .FirstOrDefault(),

                    // Zone
                    Zone = _context.Employee
                        .Where(e => e.EmployeeId == m.AssignedEmployeeId)
                        .Select(e => e.AreaZone)
                        .FirstOrDefault(),
                })
                .ToListAsync();

            // Zone Filter
            if (!string.IsNullOrWhiteSpace(zone) && zone.ToLower() != "all")
                items = items.Where(i => i.Zone == zone).ToList();

            return new PagedResultDto<object>
            {
                Data = items.Cast<object>().ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

    }
    }

























