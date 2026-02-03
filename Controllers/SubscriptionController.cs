using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Repositories.Subscription;

namespace XeniaRentalBackend.Controllers
{
    [ApiController]
    [Route("api/subscription")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionController(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }


        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _subscriptionRepository.GetMainPlansAsync();
            return Ok(plans);
        }


        [HttpPost("renew")]
        public async Task<IActionResult> RenewSubscription(
          [FromBody] RenewSubscriptionDto dto)
        {
            if (dto == null || dto.CompanyId <= 0 || dto.PlanId <= 0)
                return BadRequest("Invalid request");

            var result = await _subscriptionRepository.RenewSubscriptionAsync(dto);

            if (result == null)
                return BadRequest("Plan not found or inactive");

            return Ok(new
            {
                success = true,
                transactionId = result.TransactionId,
                paymentLink = result.PaymentLink
            });
        }

        [AllowAnonymous]
        [HttpPost("mswipe/checkStatus")]
        public async Task<IActionResult> CheckTransactionStatus(string transId)
        {
            if (string.IsNullOrWhiteSpace(transId))
                return BadRequest("TransId is required.");

            try
            {
                var result = await _subscriptionRepository.CheckTransactionStatusAsync(transId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPost("paymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus([FromQuery] string transactionRefId, [FromQuery] bool isSuccess)
        {
            if (string.IsNullOrWhiteSpace(transactionRefId))
                return BadRequest("Invalid parameters");

            var result = await _subscriptionRepository
                .UpdatePaymentStatusAsync(transactionRefId, isSuccess);

            if (!result)
                return NotFound("Transaction not found");

            return Ok(new
            {
                success = true,
                message = isSuccess
                    ? "Payment successful. Subscription activated."
                    : "Payment failed. Subscription expired."
            });
        }
    }
}
