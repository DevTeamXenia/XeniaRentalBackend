using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Ledger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountLedgerController : ControllerBase
    {
        private readonly IAccountLedgerRepository _ledgerRepository;


        public AccountLedgerController(IAccountLedgerRepository ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<XRS_Accounts>>> Get()
        {
            var ledgers = await _ledgerRepository.GetLedgers();
            if (ledgers == null || !ledgers.Any())
            {
                return NotFound(new { Status = "Error", Message = "No ledgers found." });
            }
            return Ok(new { Status = "Success", Data = ledgers });
        }
        

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_AccountLedger>>> GetLedgersByCompanyId(int companyId, [FromQuery] bool isgroup = false)
        {

            var ledgers = await _ledgerRepository.GetLedgerDetails(companyId, isgroup);
            if (ledgers == null || !ledgers.Any())
            {
                return NotFound(new { Status = "Error", Message = "No ledgers found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = ledgers });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_AccountLedger>> GetLedgerAccount(int id)
        {
            var ledgers = await _ledgerRepository.GetLedgerbyId(id);
            if (ledgers == null)
            {
                return NotFound(new { Status = "Error", Message = "Ledger not found." });
            }
            return Ok(new { Status = "Success", Data = ledgers });
        }


        [HttpPost]
        public async Task<IActionResult> CreateLedgers([FromBody] XRS_AccountLedger ledger)
        {
            if (ledger == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid ledger group." });
            }

            var createdLedger = await _ledgerRepository.CreateLedger(ledger);
            return CreatedAtAction(nameof(GetLedgerAccount), new { id = createdLedger }, new { Status = "Success", Data = createdLedger });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLedger(int id, [FromBody] XRS_AccountLedger ledger)
        {
            if (ledger == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid ledger data" });
            }

            var updated = await _ledgerRepository.UpdateLedger(id, ledger);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "ledger not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Ledger updated successfully." });
        }


    }
}
