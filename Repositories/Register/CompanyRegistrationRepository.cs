using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.Design;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.EmployeeMaster;
using XeniaTenoraBackend.DTOs;
using XeniaTenoraBackend.Models;

namespace XeniaRentalBackend.Repositories.Register
{
    public class CompanyRegistrationRepository : ICompanyRegistrationRepository
    {
        private readonly ApplicationDbContext _recontext;
        public CompanyRegistrationRepository(ApplicationDbContext recontext)
        {
            _recontext = recontext;
        }

        public async Task<int> RegisterRentalCompanyAsync(CompanyRentalRegistrationRequestDto request)
        {
            using var tx = await _recontext.Database.BeginTransactionAsync();
            try
            {
                var company = new XRS_Company
                {
                    companyName = request.companyName,
                    address = request.address,
                    email = request.email,
                    phoneNumber = request.phoneNumber,
                    pin = request.pin,
                    logo = request.logo,
                    IsActive = request.IsActive,
                    Country = request.Country,
                };

                _recontext.Company.Add(company);
                await _recontext.SaveChangesAsync();

                if (!string.IsNullOrEmpty(request.userName) && !string.IsNullOrEmpty(request.password))
                {
                    _recontext.Users.Add(new XRS_Users
                    {
                        CompanyId = company.companyID,
                        UserName = request.userName,
                        Password = request.password,
                        UserType = 1,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    });
                }

                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(14);

                _recontext.CompanySubscription.Add(new XRS_CompanySubscription
                {
                    CompanyId = company.companyID,
                    PlanId = 0,
                    SubscriptionDate = DateTime.Now,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionDays = 14,
                    SubscriptionAmount = 0,
                    SubscriptionUserCount = 2,
                    Status = "TRIAL",
                });

                foreach (var setting in request.Settings)
                {
                    _recontext.CompanySettings.Add(new XRS_CompanySettings
                    {
                        CompanyId = company.companyID,
                        KeyCode = setting.KeyCode,
                        Value = setting.Value
                    });
                }

                await _recontext.SaveChangesAsync();
                await tx.CommitAsync();
                return company.companyID;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

    }

}
