using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Company;


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




        [HttpPut("UpdateCompany/{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Models.XRS_Company company)
        {
            if (company == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid Company data" });
            }

            var updated = await _companyRepository.UpdateCompany(id, company);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "company not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Company updated successfully." });
        }


      
    }
}
