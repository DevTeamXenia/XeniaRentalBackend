using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using XeniaRentalBackend.Dtos.Report;
using XeniaRentalBackend.Repositories.Report;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;


        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] int companyId,[FromQuery] int? propertyId,[FromQuery] int? unitId, [FromQuery] bool isBedSpace = true,[FromQuery] int? bedSpaceId = null, [FromQuery] string? search = null)
        {
            var result = await _reportRepository.GetTenantOccupancyReportAsync(companyId, propertyId,unitId, bedSpaceId, isBedSpace, search);
            return Ok(result);
        }

        [HttpGet("income-expense")]
        public async Task<ActionResult<BalanceSheetResponseDto>> GetIncomeExpense( int companyId,[FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? propertyId)
        {
            try
            {
                var result = await _reportRepository.GetIncomeExpenseAsync(companyId, startDate, endDate, propertyId);

                if (result == null)
                    return NotFound("No data found for the given parameters.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
