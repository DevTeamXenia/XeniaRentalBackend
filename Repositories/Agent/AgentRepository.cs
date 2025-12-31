using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Agent
{
    public class AgentRepository : IAgentRepository
    {
        private readonly ApplicationDbContext _context;
        public AgentRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_Agent>> GetAgent()
        {

            return await _context.Agent
                .Where(u => u.AgentActive == true)
                 .Select(u => new XRS_Agent
                 {
                     AgentName = u.AgentName,
                     CompanyId = u.CompanyId,
                     AgentCode = u.AgentCode,
                     AgentActive = u.AgentActive,

                 }).ToListAsync();


        }

        public async Task<PagedResultDto<XRS_Agent>> GetAgentbyCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {

            var query = _context.Agent.AsQueryable();
            query = query.Where(u => u.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.AgentName.Contains(search));
            }
            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.AgentName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                 .Select(u => new XRS_Agent
                 {
                     AgentName = u.AgentName,
                     CompanyId = u.CompanyId,
                     AgentCode = u.AgentCode,
                     AgentActive = u.AgentActive,

                 }).ToListAsync();


            return new PagedResultDto<XRS_Agent>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

        }

        public async Task<IEnumerable<XRS_Agent>> GetAgentbyId(int agentId)
        {

            return await _context.Agent
                .Where(u => u.AgentId == agentId)
                 .Select(u => new XRS_Agent
                 {
                     AgentName = u.AgentName,
                     CompanyId = u.CompanyId,
                     AgentCode = u.AgentCode,
                     AgentActive = u.AgentActive,

                 }).ToListAsync();

        }

        public async Task<XRS_Agent> CreateAgent(XRS_Agent dtoAgent)
        {

            var agent = new XRS_Agent
            {
                AgentName = dtoAgent.AgentName,      
                AgentCode = dtoAgent.AgentCode,
                CompanyId = dtoAgent.CompanyId,
                AgentActive = dtoAgent.AgentActive,

            };
            await _context.Agent.AddAsync(agent);
            await _context.SaveChangesAsync();
            return agent;

        }
   
        public async Task<bool> UpdateAgent(int id, XRS_Agent agent)
        {
            var updateAgent = await _context.Agent.FirstOrDefaultAsync(u => u.AgentId == id);
            if (updateAgent == null) return false;

            updateAgent.AgentName = agent.AgentName;
            updateAgent.AgentCode = agent.AgentCode;
            updateAgent.AgentActive = agent.AgentActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAgent(int id)
        {
            var agent = await _context.Agent.FirstOrDefaultAsync(u => u.AgentId == id);
            if (agent == null) return false;
            agent.AgentActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    
    }
}
