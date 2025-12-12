using XeniaRentalApi.Dtos;
using XeniaRentalApi.Models;

namespace XeniaRentalApi.Repositories.MessTypes
{
    public interface IMessTypes
    {
        Task<IEnumerable<XRS_Messtypes>> GetMessTypes(int companyId);
        Task<PagedResultDto<XRS_Messtypes>> GetMessTypesByCompanyId(int companyId, int pageNumber, int pageSize);

        Task<XRS_Messtypes> CreateMessTypes(XRS_Messtypes messTypes);

        Task<bool> DeleteMessType(int id);
        Task<IEnumerable<XRS_Messtypes>> GetMessTypesbyId(int messTypeId);

        Task<bool> UpdatMessTypes(int id, XRS_Messtypes types);
    }
}
