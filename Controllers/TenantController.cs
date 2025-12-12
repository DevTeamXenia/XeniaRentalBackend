
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalApi.Dtos;
using XeniaRentalApi.Models;
using XeniaRentalApi.Repositories.Tenant;



namespace XeniaRentalApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;


        public TenantController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }


        [HttpGet("all/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Tenant>>> Get(int companyId, int? unitId = null)
        {
            var tenants = await _tenantRepository.GetTenants(companyId, unitId);
            if (tenants == null || !tenants.Any())
            {
                return NotFound(new { Status = "Error", Message = "No tenants  found." });
            }
            return Ok(new { Status = "Success", Data = tenants });
        }


        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetByCompany(int companyId, [FromQuery] bool? status, [FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var tenants = await _tenantRepository.GetTenantsByCompanyId(companyId, status, search, pageNumber, pageSize);
            return Ok(tenants);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tenant = await _tenantRepository.GetTenantWithDocumentsById(id);
            if (tenant == null)
                return NotFound();
            return Ok(tenant);
        }


        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var tenant = await _tenantRepository.GetProfileById(id);
            if (tenant == null)
                return NotFound();
            return Ok(tenant);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TenantCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTenant = await _tenantRepository.CreateTenant(dto);

            return Ok(new
            {
                Message = "Tenant created successfully",
                TenantId = createdTenant.tenantID
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TenantCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tenantRepository.UpdateTenant(id, dto);
            if (!result)
                return NotFound();

            return NoContent();
        }
     

        [HttpPost("images")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files selected.");
            }

            var result = await _tenantRepository.UploadFilesAsync(files);

            return Ok(result);
        }


        [HttpGet("image/{fileName}")]
        public async Task<IActionResult> GetFtpImage(string fileName)
        {
            var result = await _tenantRepository.GetImageFromFtpAsync(fileName);
            if (result == null)
                return NotFound();

            return File(result.Value.FileContent, result.Value.ContentType);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tenantRepository.DeleteTenant(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
