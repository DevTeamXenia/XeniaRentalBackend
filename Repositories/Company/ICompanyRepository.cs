using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Company
{
    public interface ICompanyRepository
    {

        Task<CompanyWithSubscriptionDto?> GetCompanyWithSubscriptionAsync(int companyId);
        Task<XRS_Company> UpdateCompany(int id, CompanySettingUpdateDto request);

    }
}
