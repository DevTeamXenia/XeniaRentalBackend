
using XeniaRentalApi.Dtos;

namespace XeniaRentalApi.Repositories.Units
{
    public interface IUnitRepository
    {

        Task<List<UnitDto>> GetUnits(int companyId, int? propertyId = null);
        Task<PagedResultDto<UnitDto>> GetUnitByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10);
        Task<UnitDto> GetUnitById(int unitId);
        Task<UnitDto> CreateUnit(UnitDto model);
        Task<UnitDto> UpdateUnit(UnitDto model);
        Task<bool> DeleteUnit(int unitId);


    }
}
