using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Models;
using XeniaTenoraBackend.Repositories.Area;

namespace XeniaTenoraBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasController : ControllerBase
    {
        private readonly IAreaRepository _areaRepository;

        public AreasController(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _areaRepository.GetAllAreas();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var area = await _areaRepository.GetAreaById(id);
            if (area == null) return NotFound();
            return Ok(area);
        }

        [HttpPost]
        public async Task<IActionResult> Create(XRS_Area area)
        {
            var id = await _areaRepository.AddArea(area);
            return Ok(new { AreaId = id });
        }

        [HttpPut]
        public async Task<IActionResult> Update(XRS_Area area)
        {
            var result = await _areaRepository.UpdateArea(area);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _areaRepository.DeleteArea(id);
            return Ok(result);
        }

        [HttpPost("map")]
        public async Task<IActionResult> MapArea([FromQuery] int propId, [FromQuery] int areaId)
        {
            var result = await _areaRepository.MapAreaToProperty(propId, areaId);
            return Ok(result);
        }

        [HttpGet("property/{propId}")]
        public async Task<IActionResult> GetAreasByProperty(int propId)
        {
            var data = await _areaRepository.GetAreasByProperty(propId);
            return Ok(data);
        }
    }
}
