using XeniaRentalApi.Dtos;
using XeniaRentalApi.Models;

namespace XeniaRentalApi.Repositories.Properties
{
    public interface IPropertiesRepository
    {
        Task<IEnumerable<XRS_Properties>> GetProperties(int companyId);
        Task<PagedResultDto<XRS_Properties>> GetPropertiesByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<PropertyWithUnitsDto>> GetPropertyForApp();
        Task<IEnumerable<XRS_Properties>> GetPrpoertiesbyId(int propertyId);
        Task<bool> UpDateProperties(int id, XRS_Properties properties);
        Task<XRS_Properties> CreateProperties(XRS_Properties property);
        Task<bool> DeleteProperty(int id);
    }
}
