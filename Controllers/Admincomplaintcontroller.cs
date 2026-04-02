


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Dtos.XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.AdminComplaint;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminComplaintController : ControllerBase
    {
        private readonly IAdminComplaintRepository _adminComplaintRepository;

        public AdminComplaintController(
            IAdminComplaintRepository adminComplaintRepository)
        {
            _adminComplaintRepository = adminComplaintRepository;
        }

        // ─────────────────────────────────────────
        // GET /api/AdminComplaint/newcomplaints/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("newcomplaints/{companyId}")]
        public async Task<IActionResult> GetNewComplaints(
            int companyId,
            string? search = null,
            string? zone = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _adminComplaintRepository
                .GetNewComplaints(companyId, search, zone, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No complaints found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // GET /api/AdminComplaint/inprogress/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("inprogress/{companyId}")]
        public async Task<IActionResult> GetInProgressComplaints(
            int companyId,
            string? search = null,
            string? zone = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _adminComplaintRepository
                .GetInProgressComplaints(companyId, search, zone, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No complaints found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // GET /api/AdminComplaint/closed/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("closed/{companyId}")]
        public async Task<IActionResult> GetClosedComplaints(
            int companyId,
            string? search = null,
            string? zone = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _adminComplaintRepository
                .GetClosedComplaints(companyId, search, zone, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No complaints found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // GET /api/AdminComplaint/overdue/{companyId}
        // ─────────────────────────────────────────
        [HttpGet("overdue/{companyId}")]
        public async Task<IActionResult> GetOverdueComplaints(
            int companyId,
            string? search = null,
            string? zone = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _adminComplaintRepository
                .GetOverdueComplaints(companyId, search, zone, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No overdue complaints found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // POST /api/AdminComplaint/assign
        // ─────────────────────────────────────────
        [HttpPost("assign")]
        public async Task<IActionResult> AssignComplaint(
            [FromBody] ComplaintAssignmentDto dto)
        {
            if (dto == null)
                return BadRequest(new { Status = "Error", Message = "Invalid data." });

            var result = await _adminComplaintRepository.AssignComplaint(dto);
            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // GET /api/AdminComplaint/details/{maintenanceId}
        // Grid Click → Complaint Details
        // ─────────────────────────────────────────
        [HttpGet("details/{maintenanceId}")]
        public async Task<IActionResult> GetComplaintDetails(int maintenanceId)
        {
            var result = await _adminComplaintRepository
                .GetComplaintDetails(maintenanceId);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "Complaint not found." });

            return Ok(new { Status = "Success", Data = result });
        }
        [HttpGet("report/{companyId}")]
        public async Task<IActionResult> GetMaintenanceReport(
    int companyId,
    string? search = null,
    string? status = null,
    string? zone = null,
    DateTime? dateFrom = null,
    DateTime? dateTo = null,
    int pageNumber = 1,
    int pageSize = 10)
        {
            var result = await _adminComplaintRepository
                .GetMaintenanceReport(
                    companyId, search, status, zone,
                    dateFrom, dateTo, pageNumber, pageSize);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "No data found." });

            return Ok(new { Status = "Success", Data = result });
        }


        // ─────────────────────────────────────────
        // GET /api/AdminComplaint/maintenancereport/{companyId}
        // Maintenance Activity Report
        // ─────────────────────────────────────────
        //[HttpGet("maintenancereport/{companyId}")]
        //public async Task<IActionResult> GetMaintenanceActivityReport(
        //    int companyId,
        //    [FromQuery] string? status = null,
        //    [FromQuery] string? zone = null,
        //    [FromQuery] DateTime? fromDate = null,
        //    [FromQuery] DateTime? toDate = null,
        //    [FromQuery] string? search = null,
        //    [FromQuery] int pageNumber = 1,
        //    [FromQuery] int pageSize = 10)
        //{
        //    var result = await _adminComplaintRepository
        //        .GetMaintenanceActivityReport(
        //            companyId, status, zone, fromDate, toDate, search, pageNumber, pageSize);

        //    if (result == null)
        //        return NotFound(new { Status = "Error", Message = "No data found." });

        //    return Ok(new { Status = "Success", Data = result });
        //}
    }
}

































