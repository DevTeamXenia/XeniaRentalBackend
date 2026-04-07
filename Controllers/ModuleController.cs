using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Module;
using XeniaRentalBackend.Service.Common;

namespace XeniaRentalBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // Changed to AllowAnonymous for testing, update to Authorize for production
    public class ModuleController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly JwtHelperService _jwtHelperService;

        public ModuleController(IModuleRepository moduleRepository, JwtHelperService jwtHelperService)
        {
            _moduleRepository = moduleRepository;
            _jwtHelperService = jwtHelperService;
        }

        // ─────────────────────────────────────────
        // Module CRUD
        // ─────────────────────────────────────────

        [HttpPost("create")]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto dto)
        {
            var userId = 1; // Default for now, can be updated to JWT from _jwtHelperService.GetUserId()
            var result = await _moduleRepository.CreateModuleAsync(userId, dto);

            if (result)
                return Ok(new { Status = "Success", Message = "Module created successfully." });

            return BadRequest(new { Status = "Error", Message = "Failed to create module." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] ModuleDto dto)
        {
            var userId = 1; // Default
            var result = await _moduleRepository.UpdateModuleAsync(id, dto, userId);

            if (result)
                return Ok(new { Status = "Success", Message = "Module updated successfully." });

            return NotFound(new { Status = "Error", Message = "Module not found." });
        }

        [HttpGet]
        public async Task<IActionResult> GetModules(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _moduleRepository.GetModulesAsync(search, pageNumber, pageSize);
            return Ok(new { Status = "Success", Data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModuleById(int id)
        {
            var module = await _moduleRepository.GetModuleByIdAsync(id);
            if (module == null) return NotFound(new { Status = "Error", Message = "Module not found." });

            return Ok(new { Status = "Success", Data = module });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var result = await _moduleRepository.DeleteModuleAsync(id);

            if (result)
                return Ok(new { Status = "Success", Message = "Module deleted successfully." });

            return NotFound(new { Status = "Error", Message = "Module not found." });
        }

        // ─────────────────────────────────────────
        // Plan-Module Mapping (The Mapping System)
        // ─────────────────────────────────────────

        [HttpPost("map")]
        public async Task<IActionResult> MapModuleToPlan([FromBody] XRS_PlanModuleMap dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _moduleRepository.CreateOrUpdatePlanModuleAsync(dto);

            return Ok(new
            {
                status = "success",
                message = "Plan Module Mapping saved successfully",
                data = result
            });
        }

        [HttpGet("map/{planId}")]
        public async Task<IActionResult> GetPlanModuleMappings(int planId)
        {
            var mappings = await _moduleRepository.GetPlanModuleMappingsAsync(planId);
            return Ok(new { Status = "Success", Data = mappings });
        }

        [HttpGet("sync")]
        public async Task<IActionResult> GetSyncModules()
        {
            var modules = await _moduleRepository.GetSyncModulesAsync();
            return Ok(new { Status = "Success", Data = modules });
        }
    }
}
