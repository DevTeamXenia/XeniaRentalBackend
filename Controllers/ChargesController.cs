using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Charges;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ChargesController : ControllerBase
    {
        private readonly IChargesRepository _chargesRepository;


        public ChargesController(IChargesRepository chargesRepository)
        {
            _chargesRepository = chargesRepository;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ChargesDto>>> Get(int companyId, int? propertyId = null)
        {
            var charges = await _chargesRepository.GetCharges(companyId);
            if (charges == null || !charges.Any())
            {
                return NotFound(new { Status = "Error", Message = "No charges found." });
            }
            return Ok(new { Status = "Success", Data = charges });
        }


        [HttpGet("charges/{companyId}")]
        public async Task<ActionResult<PagedResultDto<ChargesDto>>> GetChargesByCompanyId(int companyId, int? propertyId = null, string? search = null, int pageNumber = 1,
            int pageSize = 10)
        {

            var charges = await _chargesRepository.GetChargesByCompanyId(companyId, propertyId, search,pageNumber, pageSize);
            if (charges == null)
            {
                return NotFound(new { Status = "Error", Message = "No charges found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = charges });
        }


        [HttpPost]
        public async Task<IActionResult> CreateCharges([FromBody] ChargesDto charges)
        {
            if (charges == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid charges." });
            }

            var createdAccount = await _chargesRepository.CreateCharges(charges);
            return CreatedAtAction(nameof(GetChargesById), new { id = createdAccount }, new { Status = "Success", Data = createdAccount });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Charges>> GetChargesById(int id)
        {
            var charges = await _chargesRepository.GetChargesById(id);
            if (charges == null)
            {
                return NotFound(new { Status = "Error", Message = "charges not found." });
            }
            return Ok(new { Status = "Success", Data = charges });
        }



        [HttpPut("UpdateCharges/{id}")]
        public async Task<IActionResult> UpdateCharges(int id, [FromBody] Models.XRS_Charges charges)
        {
            if (charges == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid charges data" });
            }

            var updated = await _chargesRepository.UpdateCharges(id, charges);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "charges not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "charges updated successfully." });
        }

        // DELETE api/<AccountGroupController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharges(int id)
        {
            var deleted = await _chargesRepository.DeleteCharges(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "Charges not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "Charges deleted successfully." });
        }

    }
}
