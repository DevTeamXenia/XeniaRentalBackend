using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.MaintenanceCategory;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceCategoryController : ControllerBase
    {
        private readonly IMaintenanceCategoryRepository _maintenanceCategoryRepository;

        public MaintenanceCategoryController(IMaintenanceCategoryRepository maintenanceCategoryRepository)
        {
            _maintenanceCategoryRepository = maintenanceCategoryRepository;
        }

        [HttpGet("all/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_MaintenanceCategory>>> Get(int companyId)
        {
            var categories = await _maintenanceCategoryRepository.GetMaintenanceCategories(companyId);
            if (categories == null || !categories.Any())
            {
                return NotFound(new { Status = "Error", Message = "No maintenance categories found." });
            }
            return Ok(new { Status = "Success", Data = categories });
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<PagedResultDto<XRS_MaintenanceCategory>>> GetMaintenanceCategoryByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var categories = await _maintenanceCategoryRepository.GetMaintenanceCategoryByCompanyId(companyId, search, pageNumber, pageSize);
            if (categories == null)
            {
                return NotFound(new { Status = "Error", Message = "No maintenance categories found for the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = categories });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<XRS_MaintenanceCategory>>> GetMaintenanceCategory(int id)
        {
            var category = await _maintenanceCategoryRepository.GetMaintenanceCategoryById(id);
            if (category == null || !category.Any())
            {
                return NotFound(new { Status = "Error", Message = "Maintenance category not found." });
            }
            return Ok(new { Status = "Success", Data = category });
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaintenanceCategory([FromBody] MaintenanceCategoryDto category)
        {
            if (category == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid maintenance category." });
            }

            var createdCategory = await _maintenanceCategoryRepository.CreateMaintenanceCategory(category);
            return CreatedAtAction(nameof(Get), new { companyId = createdCategory.CompanyId }, new { Status = "Success", Data = createdCategory });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenanceCategory(int id, [FromBody] MaintenanceCategoryDto category)
        {
            if (category == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid maintenance category data." });
            }

            var updated = await _maintenanceCategoryRepository.UpdateMaintenanceCategory(id, category);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "Maintenance category not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Maintenance category updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenanceCategory(int id)
        {
            var deleted = await _maintenanceCategoryRepository.DeleteMaintenanceCategory(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "Maintenance category not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "Maintenance category deleted successfully." });
        }
    }
}
