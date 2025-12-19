using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Service;


namespace XeniaRentalBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;


        public ServiceController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }


        [HttpGet("all/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Service>>> Get(int companyId)
        {
            var properties = await _serviceRepository.GetServices(companyId);
            if (properties == null || !properties.Any())
            {
                return NotFound(new { Status = "Error", Message = "No service found." });
            }
            return Ok(new { Status = "Success", Data = properties });
        }

        
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<PagedResultDto<XRS_Service>>> GetServiceByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {

            var accounts = await _serviceRepository.GetServiceByCompanyId(companyId, search, pageNumber, pageSize);
            if (accounts == null)
            {
                return NotFound(new { Status = "Error", Message = "No services found the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = accounts });
        }

     
        [HttpPost]
        public async Task<IActionResult> CreateServices([FromBody] XRS_Service service)
        {
            if (service == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid service group." });
            }

            var createdService = await _serviceRepository.CreateServices(service);
            return CreatedAtAction(nameof(GetServicesbyId), new { id = createdService }, new { Status = "Success", Data = createdService });
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Service>> GetServicesbyId(int id)
        {
            var properties = await _serviceRepository.GetServicesbyId(id);
            if (properties == null)
            {
                return NotFound(new { Status = "Error", Message = "service not found." });
            }
            return Ok(new { Status = "Success", Data = properties });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServices(int id, [FromBody] XRS_Service service)
        {
            if (service == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid service data" });
            }

            var updated = await _serviceRepository.UpdateServices(id, service);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "service not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "service updated successfully." });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var deleted = await _serviceRepository.DeleteService(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "service not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "service deleted successfully." });
        }

/*
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetChargesByPropertyId(int propertyId)
        {
            var charges = await _serviceRepository.GetChargesByPropertyIdAsync(propertyId);
            if (charges == null || !charges.Any())
                return NotFound("No charges found for this property.");

            return Ok(charges);
        }*/


        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetServiceByPropertyId(int propertyId, string serviceType)
        {
            var charges = await _serviceRepository.GetServiceByPropertyIdAsync(propertyId, serviceType);
            if (charges == null || !charges.Any())
                return NotFound("No charges found for this property.");

            return Ok(charges);
        }
    }
}
