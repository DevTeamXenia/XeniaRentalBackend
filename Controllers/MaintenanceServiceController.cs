using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Repositories.MaintenanceService;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceServiceController : ControllerBase
    {
        private readonly IMaintenanceServiceRepository _serviceRepository;

        public MaintenanceServiceController(
            IMaintenanceServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // ─────────────────────────────────────────
        // GET /api/MaintenanceService/list/{maintenanceId}
        // Complaint Details Page-ൽ Service No List
        // ─────────────────────────────────────────
        [HttpGet("list/{maintenanceId}")]
        public async Task<IActionResult> GetServices(int maintenanceId)
        {
            var result = await _serviceRepository
                .GetServicesByMaintenanceId(maintenanceId);

            if (result == null || !result.Any())
                return NotFound(new { Status = "Error", Message = "No services found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // GET /api/MaintenanceService/history/{serviceId}
        // Service No Dropdown Click ചെയ്താൽ History
        // ─────────────────────────────────────────
        [HttpGet("history/{serviceId}")]
        public async Task<IActionResult> GetServiceHistory(int serviceId)
        {
            var result = await _serviceRepository
                .GetServiceHistory(serviceId);

            if (result == null)
                return NotFound(new { Status = "Error", Message = "Service not found." });

            return Ok(new { Status = "Success", Data = result });
        }

        // ─────────────────────────────────────────
        // POST /api/MaintenanceService
        // Create New Service No
        // ─────────────────────────────────────────
       
        // ─────────────────────────────────────────
        // POST /api/MaintenanceService/history
        // Create Service History
        // ─────────────────────────────────────────
        
    }
}




