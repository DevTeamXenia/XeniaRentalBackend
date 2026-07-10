using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.EmployeeMaster;
using XeniaTenoraBackend.DTOs;
using XeniaRentalBackend.Repositories.Register;

namespace XeniaRentalBackend.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ICompanyRegistrationRepository _repositoryCompanyRegistration;

        public RegisterController(ICompanyRegistrationRepository repositoryCompanyRegistration)
        {
            _repositoryCompanyRegistration = repositoryCompanyRegistration;
        }

        [HttpPost("api/rental/register")]
        public async Task<IActionResult> RegisterRentalCompanyAsync([FromBody] CompanyRentalRegistrationRequestDto request)
        {
            var companyId = await _repositoryCompanyRegistration.RegisterRentalCompanyAsync(request);

            return Ok(new
            {
                Status = "Success",
                CompanyId = companyId,
                UserName = request.userName,
                Password = request.password,
                Message = "Company registered successfully"
            });
        }
    }
}
