using Stripe;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Service
{
    public interface IServiceRepository
    {
        Task<IEnumerable<XRS_Service>> GetServices(int companyId);
        Task<PagedResultDto<XRS_Service>> GetServiceByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<XRS_Service>> GetServiceForApp();
        Task<IEnumerable<XRS_Service>> GetServicesbyId(int propertyId);
        Task<bool> UpdateServices(int id, XRS_Service service);
        Task<XRS_Service> CreateServices(XRS_Service service);
        Task<bool> DeleteService(int id);
        Task<List<XRS_Service>> GetServiceByPropertyIdAsync(int propertyId, string serviceType);
    }
}
