using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.ManageMaintenance;

using XeniaRentalBackend.Repositories.AdminComplaint;
using XeniaRentalBackend.Service.Maintenance;

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
        // GET /api/ManageMaintenance/list/{companyId}
        // Unified list API (Also triggers Socket Broadcast)
        // ─────────────────────────────────────────
        [HttpGet("list/{companyId}")]
        public async Task<IActionResult> GetMaintenanceList(
            int companyId,
            [FromQuery] string status = "pending", // "pending", "inprogress", "completed"
            [FromQuery] string? search = null,
            [FromQuery] DateTime? date = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // 1. Send via socket to any connected clients
            await _maintenanceUpdateService.SendMaintenanceUpdate(companyId, status, null, search, pageNumber, pageSize);
            
            // 2. Fetch the data directly for normal API response (so you can view it in Swagger)
            object result = null;
            switch (status.ToLower())
            {
                case "pending":
                    result = await _manageMaintenanceRepository.GetPendingList(companyId, search, date, pageNumber, pageSize);
                    break;
                case "inprogress":
                    result = await _manageMaintenanceRepository.GetInProgressList(companyId, search, date, pageNumber, pageSize);
                    break;
                case "completed":
                    result = await _manageMaintenanceRepository.GetCompletedList(companyId, search, date, pageNumber, pageSize);
                    break;
            }

            if (result == null)
                return NotFound(new { Status = "Error", Message = $"No {status} maintenance found." });

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
            await _maintenanceUpdateService.SendMaintenanceUpdate(dto.CompanyId, "pending");

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
                await _maintenanceUpdateService.SendMaintenanceUpdate(companyId.Value, dto.Status);
            }

            return Ok(new { Status = "Success", Message = $"Status updated to {dto.Status}." });
        }
    }
}






























































