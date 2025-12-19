using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.BedSpace
{
    public interface IBedSpaceRepository
    {
        Task<IEnumerable<XRS_Bedspace>> GetBedSpaces(int companyId, int? unitId = null);

        Task<PagedResultDto<XRS_Bedspace>> GetBedSpacesByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10);

        Task<XRS_Bedspace> GetBedSpaceById(int id);

        Task<XRS_Bedspace> CreateBedSpaces(BedSpaceDto bedSpace);

        Task<bool> UpdateBedSpace(int id, BedSpaceDto bedSpace);

        Task<bool> DeleteBedSpace(int id);

    }
}
