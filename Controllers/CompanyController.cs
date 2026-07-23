using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Company;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;


        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

       
        [HttpGet("{companyId}")]
        public async Task<ActionResult<XRS_Company>> GetCompanyById(int companyId)
        {
            var company = await _companyRepository.GetCompanyWithSubscriptionAsync(companyId);
            if (company == null)
            {
                return NotFound(new { Status = "Error", Message = "company not found." });
            }
            return Ok(new { Status = "Success", Data = company });
        }



        [HttpPut("UpdateCompany{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanySettingUpdateDto request)
        {
            if (request == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid Company data" });
            }
            var updated = await _companyRepository.UpdateCompany(id, request);
            return Ok(new { Status = "Success", Message = "Company updated successfully." });
        }

    }
}
