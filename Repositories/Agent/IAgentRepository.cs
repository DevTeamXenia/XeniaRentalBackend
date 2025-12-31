using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Agent
{
    public interface IAgentRepository
    {
        Task<IEnumerable<XRS_Agent>> GetAgent();

        Task<PagedResultDto<XRS_Agent>> GetAgentbyCompanyId(int companyId, string? search = null,int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<XRS_Agent>> GetAgentbyId(int Id);

        Task<XRS_Agent> CreateAgent(XRS_Agent category);

        Task<bool> UpdateAgent(int id, XRS_Agent bedSpace);

        Task<bool> DeleteAgent(int id);

    }
}
