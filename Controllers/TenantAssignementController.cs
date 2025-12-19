using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.TenantAssignment;

namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantAssignementController : ControllerBase
    {
        private readonly ITenantAssignmentRepository _tenantAssignmentRepository;

        public TenantAssignementController(ITenantAssignmentRepository tenantAssignmentRepository)
        {
            _tenantAssignmentRepository = tenantAssignmentRepository;
        }


        [HttpGet("company/all/{companyId}")]
        public async Task<IActionResult> GetByCompanyAllId(int companyId, int? unitId = null)
        {
            var data = await _tenantAssignmentRepository.GetByCompanyAllId(companyId, unitId);
            return Ok(data);
        }


        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetByCompanyId( int companyId, bool isBedSpace = false, DateTime? startDate = null, DateTime? endDate = null,int? propertyId = null, int? unitId = null, string? search = null)
        {
            var data = await _tenantAssignmentRepository.GetByCompanyIdAsync( companyId,isBedSpace,startDate, endDate, propertyId,unitId, search);

            return Ok(data);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _tenantAssignmentRepository.GetByIdAsync(id);
            if (item == null) return NotFound(new { Message = "Tenant Assignment not found." });
            return Ok(item);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TenantAssignmentCreateDto dto)
        {
            var created = await _tenantAssignmentRepository.CreateAsync(dto);
            return Ok(new { Message = "Tenant Assignment created successfully", Data = created });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TenantAssignmentCreateDto dto)
        {          
            var updated = await _tenantAssignmentRepository.UpdateAsync(id,dto);
            if (updated == null) return NotFound(new { Message = "Tenant Assignment not found." });

            return Ok(new { Message = "Tenant Assignment updated successfully", Data = updated });
        }


        [HttpGet("company/closure/{companyId}")]
        public async Task<IActionResult> GeClosure(int companyId,DateTime? startDate = null, DateTime? endDate = null,int ? propertyId = null, int? unitId = null, string? search = null)
        {
            var data = await _tenantAssignmentRepository.GeClosure(companyId, startDate, endDate, propertyId, unitId, search);
            return Ok(data);
        }


        [HttpGet("closure/{tenantAssignId}")]
        public Task<TenantAssignmentGetDto?> GetClosureById(int tenantAssignId)
        => _tenantAssignmentRepository.GetClosureById(tenantAssignId);


        [HttpPut("closure/{id}")]
        public async Task<IActionResult> CloseUpdate(int id, [FromBody] TenantClosureCreateDto dto)
        {

            var updated = await _tenantAssignmentRepository.UpdateClosureAsync(id, dto);
            if (updated == null) return NotFound(new { Message = "Tenant Assignment not found." });

            return Ok(new { Message = "Tenant Assignment updated successfully", Data = updated });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _tenantAssignmentRepository.DeleteAsync(id);
            if (!deleted) return NotFound(new { Message = "Tenant Assignment not found." });

            return Ok(new { Message = "Tenant Assignment deleted successfully" });
        }


        [HttpGet("cheque/{companyId}")]
        public async Task<IActionResult> GetChequesByCompany(int companyId,string? search = null, DateTime? startDate = null,DateTime? endDate = null, string? status = null)
        {
            var result = await _tenantAssignmentRepository.GetChequesByCompanyAsync( companyId, search, startDate, endDate, status);

            if (result == null || !result.Any())
                return NotFound(new { message = "No cheques found for this company." });

            return Ok(result);
        }



        [HttpPut("cheque/{chequeRegisterId}")]
        public async Task<IActionResult> UpdateChequePayStatus(int chequeRegisterId, string status)
        {

            var success = await _tenantAssignmentRepository.UpdateChequePayStatusAsync(chequeRegisterId, status);

            if (!success)
                return NotFound(new { message = "Cheque not found or update failed." });

            return Ok(new { message = "Payment status updated successfully." });
        }

    }
}
