using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.AccountGroups;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountGroupController : ControllerBase
    {
        private readonly IAccountGroupRepository _accountGroupRepository;

   
        public AccountGroupController(IAccountGroupRepository accountGroupRepository)
        {
            _accountGroupRepository = accountGroupRepository;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<XRS_AccountGroup>>> Get()
        {
            var accountGroups = await _accountGroupRepository.GetAccountGroups();
            if (accountGroups == null || !accountGroups.Any())
            {
                return NotFound(new { Status = "Error", Message = "No accountGroups found." });
            }
            return Ok(new { Status = "Success", Data = accountGroups });
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_AccountGroup>>> GetAccountGroupsByCompanyId(int companyId)
        {
            
            var accountGroups = await _accountGroupRepository.GetAccountGroupsByCompanyId(companyId);
            if (accountGroups == null || !accountGroups.Any())
            {
                return NotFound(new { Status = "Error", Message = "No accountGroups found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = accountGroups });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_AccountGroup>> GetaccountGroup(int id)
        {
            var accountGroup = await _accountGroupRepository.GetAccountGroupById(id);
            if (accountGroup == null)
            {
                return NotFound(new { Status = "Error", Message = "AccountGroup not found." });
            }
            return Ok(new { Status = "Success", Data = accountGroup });
        }


        [HttpPost]
        public async Task<IActionResult> CreateAccountGroups([FromBody] XRS_AccountGroup accountGroup)
        {
            if (accountGroup == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid account group." });
            }

            var createdAccountGroup = await _accountGroupRepository.CreateAccountGroups(accountGroup);
            return CreatedAtAction(nameof(GetaccountGroup), new { id = createdAccountGroup }, new { Status = "Success", Data = createdAccountGroup });
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccountGroup(int id, [FromBody] Models.XRS_AccountGroup account)
        {
            if (account == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid account data" });
            }

            var updated = await _accountGroupRepository.UpdateAccountGroup(id, account);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "account not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Account Group updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var deleted = await _accountGroupRepository.DeleteAccountGroup(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "AccountGroup not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "Account Group deleted successfully." });
        }

       
    }
}
