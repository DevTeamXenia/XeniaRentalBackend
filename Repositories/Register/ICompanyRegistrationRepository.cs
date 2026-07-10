using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Models.Rental;
using XeniaTenoraBackend.DTOs;

namespace XeniaRentalBackend.Repositories.Register
{
    public interface ICompanyRegistrationRepository
    {
        Task<int> RegisterRentalCompanyAsync(CompanyRentalRegistrationRequestDto request);
    }
}
