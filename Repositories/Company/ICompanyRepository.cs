using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Company
{
    public interface ICompanyRepository
    {

        Task<CompanyWithSubscriptionDto?> GetCompanyWithSubscriptionAsync(int companyId);

        Task<bool> UpdateCompany(int id, XRS_Company charges);

    }
}
