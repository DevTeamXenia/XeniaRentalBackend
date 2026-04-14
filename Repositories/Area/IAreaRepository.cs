using XeniaRentalBackend.Models;

namespace XeniaTenoraBackend.Repositories.Area
{
    public interface IAreaRepository
    {
        Task<IEnumerable<XRS_Area>> GetAllAreas();
        Task<XRS_Area> GetAreaById(int id);
        Task<int> AddArea(XRS_Area area);
        Task<bool> UpdateArea(XRS_Area area);
        Task<bool> DeleteArea(int id);

        Task<bool> MapAreaToProperty(int propId, List<int> areaIds);
        Task<IEnumerable<XRS_Area>> GetAreasByProperty(int propId);
    }
}
