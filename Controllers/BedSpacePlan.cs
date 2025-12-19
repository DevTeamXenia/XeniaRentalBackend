using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.BedSpacePlan;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BedSpacePlan : ControllerBase
    {
        private readonly IBedSpacePlanRepository _bedSpacePlanRepository;


        public BedSpacePlan(IBedSpacePlanRepository bedSpacePlanRepository)
        {
            _bedSpacePlanRepository = bedSpacePlanRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _bedSpacePlanRepository.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _bedSpacePlanRepository.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetByCompanyId(int companyId)
        {
            var data = await _bedSpacePlanRepository.GetByCompanyIdAsync(companyId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] XRS_BedSpacePlan model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _bedSpacePlanRepository.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.bedPlanID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] XRS_BedSpacePlan model)
        {
            if (id != model.bedPlanID)
                return BadRequest("Bed ID mismatch");

            var updated = await _bedSpacePlanRepository.UpdateAsync(model);

            if (!updated)
                return NotFound();
         
            return Ok(new { Message = "Bed Space Plan updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bedSpacePlanRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound();


            return Ok(new { Message = "Bed Space Plan deleted successfully" });
        }

    }
}
