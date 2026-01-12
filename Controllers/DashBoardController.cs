using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Repositories.Dashboard;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashboardRepsitory _dashboardRepsitory;



        public DashBoardController(IDashboardRepsitory dashboardRepsitory)
        {
            _dashboardRepsitory = dashboardRepsitory;
        }


        [HttpGet("dashboard/rent")]
        public async Task<IActionResult> GetRentDashboard(int companyid, DateTime fromDate, DateTime toDate)
        {
            var data = await _dashboardRepsitory.GetRentDashboardAsync(companyid,fromDate, toDate);
            return Ok(data);
        }

        [HttpGet("rent/monthly")]
        public async Task<IActionResult> GetMonthlyRevenue(int companyid,[FromQuery] int year)
        {
            var revenue = await _dashboardRepsitory.GetMonthlyRentRevenueAsync(companyid,year);
            return Ok(revenue);
        }

        [HttpGet("home/{unitId}")]
        public async Task<IActionResult> GetTenantPayments(int unitId)
        {
            var result = await _dashboardRepsitory.GetTenantPaymentsAsync(unitId);
            return Ok(result);
        }

    }
}
