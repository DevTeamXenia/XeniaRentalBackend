using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.ManageMaintenance;

using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceRepository _manageMaintenanceRepository;
        //private readonly IAdminComplaintRepository _adminComplaintRepository;

        public MaintenanceController(IMaintenanceRepository manageMaintenanceRepository)
        {
            _manageMaintenanceRepository = manageMaintenanceRepository;
            //_/*adminComplaintRepository = adminComplaintRepository;*/

        }


        #region CATEGORY

        [HttpGet("category/all/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_MaintenanceCategory>>> GetCategories(int companyId)
        {
            var categories = await _manageMaintenanceRepository.GetMaintenanceCategories(companyId);
            if (categories == null || !categories.Any())
            {
                return NotFound(new { Status = "Error", Message = "No maintenance categories found." });
            }
            return Ok(new { Status = "Success", Data = categories });
        }

        [HttpGet("category/company/{companyId}")]
        public async Task<ActionResult<PagedResultDto<XRS_MaintenanceCategory>>> GetMaintenanceCategoryByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var categories = await _manageMaintenanceRepository.GetMaintenanceCategoryByCompanyId(companyId, search, pageNumber, pageSize);
            if (categories == null)
            {
                return NotFound(new { Status = "Error", Message = "No maintenance categories found for the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = categories });
        }

        [HttpGet("category/{id}")]
        public async Task<ActionResult<IEnumerable<XRS_MaintenanceCategory>>> GetMaintenanceCategory(int id)
        {
            var category = await _manageMaintenanceRepository.GetMaintenanceCategoryById(id);
            if (category == null || !category.Any())
            {
                return NotFound(new { Status = "Error", Message = "Maintenance category not found." });
            }
            return Ok(new { Status = "Success", Data = category });
        }

        [HttpPost("category")]
        public async Task<IActionResult> CreateMaintenanceCategory([FromBody] MaintenanceCategoryDto category)
        {
            if (category == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid maintenance category." });
            }

            var createdCategory = await _manageMaintenanceRepository.CreateMaintenanceCategory(category);
            return Ok(new { Status = "Success", Data = createdCategory });
        }

        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateMaintenanceCategory(int id, [FromBody] MaintenanceCategoryDto category)
        {
            if (category == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid maintenance category data." });
            }

            var updated = await _manageMaintenanceRepository.UpdateMaintenanceCategory(id, category);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "Maintenance category not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "Maintenance category updated successfully." });
        }

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteMaintenanceCategory(int id)
        {
            var deleted = await _manageMaintenanceRepository.DeleteMaintenanceCategory(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "Maintenance category not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "Maintenance category deleted successfully." });
        }
    

#endregion


        //[HttpPost("service")]
        //public async Task<IActionResult> Create(MaintenanceDto dto)
        //{
        //    if (dto == null)
        //        return BadRequest(new { Status = "Error", Message = "Invalid data." });

        //    var created = await _manageMaintenanceRepository.CreateMaintenance(dto);


        //    return Ok(new { Status = "Success", Data = created });
        //}


        //[HttpPut("service/status/{maintainceId}")]
        //public async Task<IActionResult> UpdateStatus(int maintainceId, int? employeeId,string status)
        //{
        //    var updated = await _manageMaintenanceRepository .UpdateMaintenanceStatus(maintainceId, employeeId, status);

        //    if (!updated)
        //        return NotFound(new { Status = "Error", Message = "Maintenance not found." });



        //    return Ok(new { Status = "Success", Message = $"Status updated to {status}." });
        //}
    }
 }































































