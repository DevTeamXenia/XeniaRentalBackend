using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.MessTypes;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MessTypesController : ControllerBase
    {
        private readonly IMessTypes _messTypesRepository;


        public MessTypesController(IMessTypes messTypesRepository)
        {
            _messTypesRepository = messTypesRepository;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<XRS_Messtypes>>> GetMessTypes(int companyId)
        {
            var messTypes = await _messTypesRepository.GetMessTypes(companyId);
            if (messTypes == null || !messTypes.Any())
            {
                return NotFound(new { Status = "Error", Message = "No messtypes found." });
            }
            return Ok(new { Status = "Success", Data = messTypes });
        }


        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Messtypes>>> GetMesstypesByCompanyId(int companyId,int pageNumber = 1,int pageSize = 10)
        {

            var messtypes = await _messTypesRepository.GetMessTypesByCompanyId(companyId,pageNumber,pageSize);
            if (messtypes == null)
            {
                return NotFound(new { Status = "Error", Message = "No messtypes found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = messtypes });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Messtypes>> GetMessTypesById(int id)
        {
            var messTypes = await _messTypesRepository.GetMessTypesbyId(id);
            if (messTypes == null)
            {
                return NotFound(new { Status = "Error", Message = "messTypes not found." });
            }
            return Ok(new { Status = "Success", Data = messTypes });
        }


        [HttpPost]
        public async Task<IActionResult> CreateMesstypes([FromBody] XRS_Messtypes messtypes)
        {
            if (messtypes == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid mess types." });
            }

            var createdAccount = await _messTypesRepository.CreateMessTypes(messtypes);
            return CreatedAtAction(nameof(GetMessTypesById), new { id = createdAccount }, new { Status = "Success", Data = createdAccount });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessTypes(int id, [FromBody] XRS_Messtypes messTypes)
        {
            if (messTypes == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid messtypes data" });
            }

            var updated = await _messTypesRepository.UpdatMessTypes(id, messTypes);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "messtypes not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "MessTypes updated successfully." });
        }

    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMesstypes(int id)
        {
            var deleted = await _messTypesRepository.DeleteMessType(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "MessTypes not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "MessTypes deleted successfully." });
        }
    }
}
