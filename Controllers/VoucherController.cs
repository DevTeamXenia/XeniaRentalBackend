using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Voucher;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;


        public VoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }


        [HttpPost]
        public async Task<ActionResult<XRS_Voucher>> CreateVoucher([FromBody] VoucherDto dto)
        {
            var voucher = await _voucherRepository.CreateVoucherAsync(dto);
            return CreatedAtAction(nameof(GetVoucherById), new { id = voucher.VoucherID }, voucher);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Voucher>> GetVoucherById(int id)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(id);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }


        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Voucher>>> GetAllVouchers(int companyId, string? search = null)
        {
            var vouchers = await _voucherRepository.GetAllVouchersAsync(companyId, search);
            return Ok(vouchers);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<XRS_Voucher>> UpdateVoucher(int id, [FromBody] VoucherDto dto)
        {
            var updatedVoucher = await _voucherRepository.UpdateVoucherAsync(id, dto);
            if (updatedVoucher == null) return NotFound();
            return Ok(updatedVoucher);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVoucher(int id)
        {
            var deleted = await _voucherRepository.DeleteVoucherAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }


        [HttpGet("rent/initiate/{month:int}/{year:int}")]
        public async Task<IActionResult> GetTenantCharges(int month, int year)
        {
            var data = await _voucherRepository.GetTenantChargesByMonthAsync(month, year);
            return Ok(data);
        }


        [HttpPost("rent/initiate")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var voucher = await _voucherRepository.CreateIntiateAsync(request);
                return Ok(new { message = "Voucher created successfully", voucherId = voucher.VoucherID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



    }
}
