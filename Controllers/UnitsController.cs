
using Microsoft.AspNetCore.Mvc;
using XeniaRentalApi.Dtos;
using XeniaRentalApi.Repositories.Units;

namespace XeniaRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitRepository _unitRepository;

        public UnitsController(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        [HttpGet("all/{companyId}")]
        public async Task<IActionResult> GetUnits(int companyId, int? propertyId = null)
        {
            var result = await _unitRepository.GetUnits(companyId, propertyId);
            return Ok(result);
        }


        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetUnitsByCompany(int companyId, string? search, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _unitRepository.GetUnitByCompanyId(companyId, search, pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{unitId}")]
        public async Task<IActionResult> GetUnit(int unitId)
        {
            var unit = await _unitRepository.GetUnitById(unitId);
            if (unit == null) return NotFound();
            return Ok(unit);
        }


        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromBody] UnitDto model)
        {
            var unit = await _unitRepository.CreateUnit(model);
            return CreatedAtAction(nameof(GetUnit), new { unitId = unit.UnitId }, unit);
        }


        [HttpPut("{unitId}")]
        public async Task<IActionResult> UpdateUnit(int unitId, [FromBody] UnitDto model)
        {
            if (unitId != model.UnitId) return BadRequest();
            var updated = await _unitRepository.UpdateUnit(model);
            if (updated == null) return NotFound();
            return Ok(updated);
        }


        [HttpDelete("{unitId}")]
        public async Task<IActionResult> DeleteUnit(int unitId)
        {
            var deleted = await _unitRepository.DeleteUnit(unitId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
