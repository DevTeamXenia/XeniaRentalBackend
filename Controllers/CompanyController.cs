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
        private readonly ICompanyRepsitory _companyRepository;


        public CompanyController(ICompanyRepsitory companyRepository)
        {
            _companyRepository = companyRepository;
        }


        [HttpGet("all/companies")]
        public async Task<ActionResult<IEnumerable<XRS_Company>>> Get(int pageNumber = 1, int pageSize = 10)
        {
            var companies = await _companyRepository.GetCompanies(pageNumber, pageSize);
            if (companies == null || !companies.Any())
            {
                return NotFound(new { Status = "Error", Message = "No companies found." });
            }
            return Ok(new { Status = "Success", Data = companies });
        }

      

        [HttpPost]
        public async Task<IActionResult> CreateCompanies([FromBody] XRS_Company company)
        {
            if (company == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid company." });
            }

            var createdCompany = await _companyRepository.CreateCompany(company);
            return CreatedAtAction(nameof(GetCompanyById), new { id = createdCompany }, new { Status = "Success", Data = createdCompany });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Company>> GetCompanyById(int id)
        {
            var company = await _companyRepository.GetCompanybyId(id);
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

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var deleted = await _companyRepository.DeleteCompany(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "company not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "company deleted successfully." });
        }

      
    }
}
