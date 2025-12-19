using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.MessDetails;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MessAttendanceController : ControllerBase
    {
        private readonly IMessAttendancesRepository _messDetailsRepository;


        public MessAttendanceController(IMessAttendancesRepository messDetailsRepository)
        {
            _messDetailsRepository = messDetailsRepository;
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyAttendance(int companyId,int month, int year,int? propertyId = null, int? unitId = null, int? bedSpaceid = null, string? search= null)
        {
            var data = await _messDetailsRepository.GetMonthlyMessAttendanceAsync(companyId, month, year, propertyId, unitId, bedSpaceid, search);
            return Ok(data);
        }


        [HttpPost("mark")]
        public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid attendance record.");

            var success = await _messDetailsRepository.MarkAttendanceAsync(dto);

            if (success)
                return Ok(new { Message = "Attendance marked successfully." });

            return StatusCode(500, "Error marking attendance.");
        }


    }
}
