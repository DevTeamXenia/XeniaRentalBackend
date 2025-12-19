using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.BedSpace;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BedSpaceController : ControllerBase
    {
        private readonly IBedSpaceRepository _bedSpaceRepository;


        public BedSpaceController(IBedSpaceRepository bedSpaceRepository)
        {
            _bedSpaceRepository = bedSpaceRepository;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<XRS_Bedspace>>> Get(int companyId, int? unitId = null)
        {
            var bedSpaces = await _bedSpaceRepository.GetBedSpaces(companyId, unitId);
            if (bedSpaces == null || !bedSpaces.Any())
            {
                return NotFound(new { Status = "Error", Message = "No bespaces found." });
            }
            return Ok(new { Status = "Success", Data = bedSpaces });
        }


        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<PagedResultDto<XRS_Bedspace>>> GetBedSpacesByCompanyId(int companyId, string? search = null, int pageNumber = 1,int pageSize = 10)
        {

            var bedSpaces = await _bedSpaceRepository.GetBedSpacesByCompanyId(companyId, search, pageNumber,pageSize);
            if (bedSpaces == null)
            {
                return NotFound(new { Status = "Error", Message = "No bedspaces found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = bedSpaces });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Bedspace>> GetBedSpace(int id)
        {
            var accountGroup = await _bedSpaceRepository.GetBedSpaceById(id);
            if (accountGroup == null)
            {
                return NotFound(new { Status = "Error", Message = "BedSpace not found." });
            }
            return Ok(new { Status = "Success", Data = accountGroup });
        }


        [HttpPost]
        public async Task<IActionResult> CreateBedSpaces([FromBody] BedSpaceDto bedSpace)
        {
            if (bedSpace == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid bedpace group." });
            }

            var createdbedSpace = await _bedSpaceRepository.CreateBedSpaces(bedSpace);
            return CreatedAtAction(nameof(GetBedSpace), new { id = createdbedSpace }, new { Status = "Success", Data = createdbedSpace });
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBedSpace(int id, [FromBody] BedSpaceDto account)
        {
            if (account == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid bedspace data" });
            }

            var updated = await _bedSpaceRepository.UpdateBedSpace(id, account);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "bedspace not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Bedspace updated successfully." });
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBedSpace(int id)
        {
            var deleted = await _bedSpaceRepository.DeleteBedSpace(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "BedSpace not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "BedSpace deleted successfully." });
        }
     
    }
}
