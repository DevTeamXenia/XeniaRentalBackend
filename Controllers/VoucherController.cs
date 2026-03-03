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

        [HttpGet("company/expenseVoucher/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Voucher>>> GetAllExpenseVouchers(int companyId, DateTime? fromDate, DateTime? toDate, int? propertyId, string? voucherStatus, string? search)
        {
            var vouchers = await _voucherRepository.GetAllExpenseVouchersAsync(companyId, fromDate, toDate, propertyId, voucherStatus, search);
            return Ok(vouchers);
        }


        [HttpPost("expenseVoucher")]
        public async Task<ActionResult<XRS_Voucher>> CreateVoucher([FromBody] VoucherDto dto)
        {
            var voucher = await _voucherRepository.CreateVoucherAsync(dto);
            return CreatedAtAction(nameof(GetVoucherById), new { id = voucher.VoucherID }, voucher);
        }


        [HttpGet("expenseVoucher/{id}")]
        public async Task<ActionResult<XRS_Voucher>> GetVoucherById(int id)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(id);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }


        [HttpPut("expenseVoucher/{id}")]
        public async Task<ActionResult<XRS_Voucher>> UpdateVoucher(int id, [FromBody] VoucherDto dto)
        {
            var updatedVoucher = await _voucherRepository.UpdateVoucherAsync(id, dto);
            if (updatedVoucher == null) return NotFound();
            return Ok(updatedVoucher);
        }


        [HttpGet("collectionStatus/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Voucher>>> GetAllVouchers(int companyId, DateTime? fromDate, DateTime? toDate, int? propertyId, int? unitId, string? voucherStatus, string? search)
        {
            var vouchers = await _voucherRepository.GetAllVouchersAsync(companyId, fromDate, toDate, propertyId, unitId, voucherStatus, search);
            return Ok(vouchers);
        }

        [HttpPut("collectionStatus/{id}")]
        public async Task<IActionResult> UpdatePaymentVoucher(int id, [FromBody] VoucherDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid voucher data.");

            try
            {
                var result = await _voucherRepository.UpdatePaymentVoucherAsync(id,dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        [HttpGet("rent/initiate/{companyId:int}/{month:int}/{year:int}")]
        public async Task<IActionResult> GetTenantCharges(int companyId , int month, int year, int? propertyId = null, int? unitId = null, int? bedSpaceId = null, string? search = null)
        {
            var data = await _voucherRepository.GetTenantChargesByMonthAsync(companyId, month, year, propertyId , unitId , bedSpaceId, search);
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


        [HttpPut("rent/update{voucherId}")]
        public async Task<IActionResult> UpdateVoucher( int voucherId, [FromBody] VoucherCreateRequest request)
        {
            var updatedVoucher = await _voucherRepository.UpdateAsync(voucherId, request);
            return Ok(updatedVoucher);
        }


    }
}
