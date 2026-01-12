using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Agent;


[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AgentController : ControllerBase
{
    private readonly IAgentRepository _agentRepository;
    public AgentController(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }


    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<XRS_Agent>>> Get()
    {
        var agent = await _agentRepository.GetAgent();
        if (agent == null || !agent.Any())
        {
            return NotFound(new { Status = "Error", Message = "No agent found." });
        }
        return Ok(new { Status = "Success", Data = agent });
    }

 
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<PagedResultDto<XRS_Agent>>> GetAgentByCompanyId(int companyId, string? search = null, int pageNumber = 1,int pageSize = 10)
    {

        var agent = await _agentRepository.GetAgentbyCompanyId(companyId, search, pageNumber, pageSize);
        if (agent == null)
        {
            return NotFound(new { Status = "Error", Message = "No agent found the given Company ID." });
        }
        return Ok(new { Status = "Success", Data = agent });
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<XRS_Agent>> GetAgent(int id)
    {
        var agent = await _agentRepository.GetAgentbyId(id);
        if (agent == null)
        {
            return NotFound(new { Status = "Error", Message = "Agent not found." });
        }
        return Ok(new { Status = "Success", Data = agent });
    }



    [HttpPost]
    public async Task<IActionResult> CreateAgent([FromBody] XRS_Agent agent)
    {
        if (agent == null)
        {
            return BadRequest(new { Status = "Error", Message = "Invalid agent." });
        }

        var createdAgent = await _agentRepository.CreateAgent(agent);
        return CreatedAtAction(nameof(GetAgent), new { id = createdAgent }, new { Status = "Success", Data = createdAgent });
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAgent(int id, [FromBody] XRS_Agent agent)
    {
        if (agent == null)
        {
            return BadRequest(new { Status = "Error", Message = "Invalid agent data" });
        }

        var updated = await _agentRepository.UpdateAgent(id, agent);
        if (!updated)
        {
            return NotFound(new { Status = "Error", Message = "agent not found or update failed." });
        }

        return Ok(new { Status = "Success", Message = "Agent updated successfully." });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAgent(int id)
    {
        var deleted = await _agentRepository.DeleteAgent(id);
        if (!deleted)
        {
            return NotFound(new { Status = "Error", Message = "Agent not found or delete failed." });
        }

        return Ok(new { Status = "Success", Message = "Agent deleted successfully." });
    }

   
}