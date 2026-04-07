using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.EmployeeMaster
{
    public class EmployeeMasterRepository : IEmployeeMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────
        // GET ALL — for dropdowns in other modules
        // ─────────────────────────────────────────
        public async Task<IEnumerable<XRS_Employee>> GetAllEmployees(int companyId)
        {
            return await _context.Employee
                .Where(e => e.CompanyId == companyId && e.IsActive == true)
                .Select(e => new XRS_Employee
                {
                    EmployeeId = e.EmployeeId,
                    CompanyId = e.CompanyId,
                    EmployeeCode = e.EmployeeCode,
                    Name = e.Name,
                    Department = e.Department,
                    Specialization = e.Specialization,
                    AreaZone = e.AreaZone,
                    MobileNumber = e.MobileNumber,
                    IsActive = e.IsActive
                    // Password NOT returned
                })
                .ToListAsync();
        }

        // ─────────────────────────────────────────
        // GET PAGED — for listing page with search
        // ─────────────────────────────────────────
        public async Task<PagedResultDto<XRS_Employee>> GetEmployeesByCompanyId(
            int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Employee
                .Where(e => e.CompanyId == companyId)
                .AsQueryable();

            // Search by name or employee code
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Name.Contains(search) ||
                    e.EmployeeCode.Contains(search) ||
                    e.Department.Contains(search)
                );
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new XRS_Employee
                {
                    EmployeeId = e.EmployeeId,
                    CompanyId = e.CompanyId,
                    EmployeeCode = e.EmployeeCode,
                    Name = e.Name,
                    Department = e.Department,
                    Specialization = e.Specialization,
                    AreaZone = e.AreaZone,
                    MobileNumber = e.MobileNumber,
                    Password = e.Password, // hashed
                    IsActive = e.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<XRS_Employee>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        // ─────────────────────────────────────────
        // GET BY ID — for edit modal pre-fill
        // ─────────────────────────────────────────
        //public async Task<XRS_EmployeeMaster?> GetEmployeeById(int employeeId)
        //{
        //    return await _context.EmployeeMasters
        //        .Where(e => e.EmployeeId == employeeId)
        //        .Select(e => new XRS_EmployeeMaster
        //        {
        //            EmployeeId = e.EmployeeId,
        //            CompanyId = e.CompanyId,
        //            EmployeeCode = e.EmployeeCode,
        //            Name = e.Name,
        //            Department = e.Department,
        //            Specialization = e.Specialization,
        //            AreaZone = e.AreaZone,
        //            MobileNumber = e.MobileNumber,
        //            IsActive = e.IsActive
        //            // Password NOT returned for security
        //        })
        //        .FirstOrDefaultAsync();
        //}

        // ─────────────────────────────────────────
        // CREATE — Save button in modal
        // ─────────────────────────────────────────
        public async Task<XRS_Employee> CreateEmployee(EmployeeMasterDto dto)
        {
            var employee = new XRS_Employee
            {
                CompanyId = dto.CompanyId,
                EmployeeCode = dto.EmployeeCode,
                Name = dto.Name,
                Department = dto.Department,
                Specialization = dto.Specialization,
                AreaZone = string.Join(",", dto.AreaZone), // ["North Zone","West Zone"] → "North Zone,West Zone"
                MobileNumber = dto.MobileNumber,
                 Password = dto.Password,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Employee.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }



        public async Task<XRS_Employee?> GetEmployeeById(int employeeId)
        {
            return await _context.Employee
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new XRS_Employee
                {
                    EmployeeId = e.EmployeeId,
                    CompanyId = e.CompanyId,
                    EmployeeCode = e.EmployeeCode,
                    Name = e.Name,
                    Department = e.Department,
                    Specialization = e.Specialization,
                    AreaZone = e.AreaZone,
                    MobileNumber = e.MobileNumber,
                    IsActive = e.IsActive
                    // Password NOT returned
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateEmployee(int id, EmployeeMasterDto dto)
        {
            var employee = await _context.Employee
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return false;

            employee.EmployeeCode = dto.EmployeeCode;
            employee.Name = dto.Name;
            employee.Department = dto.Department;
            employee.Specialization = dto.Specialization;
            employee.AreaZone = string.Join(",", dto.AreaZone);
            employee.MobileNumber = dto.MobileNumber;
            employee.IsActive = dto.IsActive;
            employee.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                employee.Password = dto.Password;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}