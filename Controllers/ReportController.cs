using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    }
}
