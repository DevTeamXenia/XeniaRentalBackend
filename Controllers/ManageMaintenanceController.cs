using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.ManageMaintenance;

using XeniaRentalBackend.Repositories.AdminComplaint;
using XeniaRentalBackend.Service.Socket;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ManageMaintenanceController : ControllerBase
    {
        private readonly IManageMaintenanceRepository _manageMaintenanceRepository;
        private readonly IAdminComplaintRepository _adminComplaintRepository;
        private readonly IMaintenanceUpdateService _maintenanceUpdateService;

        public ManageMaintenanceController(
            IManageMaintenanceRepository manageMaintenanceRepository,
            IAdminComplaintRepository adminComplaintRepository,
            IMaintenanceUpdateService maintenanceUpdateService)
        {
            _manageMaintenanceRepository = manageMaintenanceRepository;
            _adminComplaintRepository = adminComplaintRepository;
            _maintenanceUpdateService = maintenanceUpdateService;
        }

        // ─────────────────────────────────────────
        // GET /api/ManageMaintenance/pending/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("pending/{companyId}")]
        public async Task<IActionResult> GetPendingList(
            int companyId,
            string? search = null,
            DateTime? date = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _manageMaintenanceRepository
                .GetPendingList(companyId, search, date, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No pending maintenance found." });

            return Ok(new { Status = "Success", Data = result });
        }

     
   
        // ─────────────────────────────────────────
        // GET /api/ManageMaintenance/inprogress/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("inprogress/{companyId}")]
        public async Task<IActionResult> GetInProgressList(
            int companyId,
            string? search = null,
            DateTime? date = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _manageMaintenanceRepository
                .GetInProgressList(companyId, search, date, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No in-progress maintenance found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // GET /api/ManageMaintenance/completed/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("completed/{companyId}")]
        public async Task<IActionResult> GetCompletedList(
            int companyId,
            string? search = null,
            DateTime? date = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _manageMaintenanceRepository
                .GetCompletedList(companyId, search, date, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No completed maintenance found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // POST /api/ManageMaintenance
        // Create New Request + Multiple Photos
        // ─────────────────────────────────────────
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ManageMaintenanceDto dto)
        {
            if (dto == null)
                return BadRequest(new { Status = "Error", Message = "Invalid data." });

            var created = await _manageMaintenanceRepository.CreateMaintenance(dto);

            // Broadcast the new maintenance update immediately via socket
            await _maintenanceUpdateService.SendMaintenanceUpdate(dto.CompanyId);

            return Ok(new { Status = "Success", Data = created });
        }

        // ─────────────────────────────────────────
        // PUT /api/ManageMaintenance/{id}/status
        // Update Status — Pending/InProgress/Completed
        // ─────────────────────────────────────────
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] UpdateMaintenanceStatusDto dto)
        {
            if (dto == null)
                return BadRequest(new { Status = "Error", Message = "Invalid status data." });

            var companyId = await _manageMaintenanceRepository.GetMaintenanceCompanyIdAsync(id);

            var updated = await _manageMaintenanceRepository
                .UpdateMaintenanceStatus(id, dto);

            if (!updated)
                return NotFound(new { Status = "Error", Message = "Maintenance not found." });

            if (companyId.HasValue)
            {
                // Broadcast updated list to the specific company connected clients
                await _maintenanceUpdateService.SendMaintenanceUpdate(companyId.Value);
            }

            return Ok(new { Status = "Success", Message = $"Status updated to {dto.Status}." });
        }
    }
}






























































