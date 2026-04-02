using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.EmployeeMaster;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeMasterRepository _employeeMasterRepository;
        private readonly ApplicationDbContext _context;

        public EmployeeController(
       IEmployeeMasterRepository employeeMasterRepository,
       ApplicationDbContext context)
        {
            _employeeMasterRepository = employeeMasterRepository;
            _context = context;
        }
        // GET /api/EmployeeMaster/company/{companyId}
        // Listing Page + Search
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<PagedResultDto<XRS_Employee>>> GetByCompany(
            int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var employees = await _employeeMasterRepository.GetEmployeesByCompanyId(
                companyId, search, pageNumber, pageSize);

            if (employees == null)
                return NotFound(new { Status = "Error", Message = "No employees found." });

            return Ok(new { Status = "Success", Data = employees });
        }

        // POST /api/EmployeeMaster
        // Create New Employee
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeMasterDto dto)
        {
            if (dto == null)
                return BadRequest(new { Status = "Error", Message = "Invalid employee data." });

            var created = await _employeeMasterRepository.CreateEmployee(dto);
            return Ok(new { Status = "Success", Data = created });
        }
        // GET /api/Employee/zones/{companyId}
        [HttpGet("zones/{companyId}")]
        public async Task<IActionResult> GetZones(int companyId)
        {
            var employees = await _context.Employee
                .Where(e => e.CompanyId == companyId && e.IsActive == true)
                .Select(e => e.AreaZone)
                .ToListAsync();

            var zones = employees
                .Where(z => !string.IsNullOrEmpty(z))
                .SelectMany(z => z.Split(','))
                .Select(z => z.Trim())
                .Distinct()
                .OrderBy(z => z)
                .ToList();

            return Ok(new { Status = "Success", Data = zones });
        }


        // GET /api/Employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeMasterRepository.GetEmployeeById(id);

            if (employee == null)
                return NotFound(new { Status = "Error", Message = "Employee not found." });

            return Ok(new { Status = "Success", Data = employee });
        }

        // PUT /api/Employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeMasterDto dto)
        {
            if (dto == null)
                return BadRequest(new { Status = "Error", Message = "Invalid employee data." });

            var updated = await _employeeMasterRepository.UpdateEmployee(id, dto);

            if (!updated)
                return NotFound(new { Status = "Error", Message = "Employee not found." });

            return Ok(new { Status = "Success", Message = "Employee updated successfully." });
        }
    }
}
