using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Repositories.Subscription;
using XeniaRentalBackend.Service.Common;
using Stripe;

namespace XeniaRentalBackend.Controllers
{
    [ApiController]
    [Route("api/subscription")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly JwtHelperService _jwtHelperService;

        public SubscriptionController(ISubscriptionRepository subscriptionRepository, JwtHelperService jwtHelperService)
        {
            _subscriptionRepository = subscriptionRepository;
            _jwtHelperService = jwtHelperService;
        }


        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var companyId = _jwtHelperService.GetCompanyId();

            if (companyId == 0)
            {
                return Unauthorized(new { Status = "Error", Message = "Unauthorized: CompanyId missing in token." });
            }

            var plans = await _subscriptionRepository.GetMainPlansAsync(companyId);

            return Ok(plans);
        }

        [HttpGet("addonPlans")]
        public async Task<IActionResult> GetAddonPlans()
        {
            var companyId = _jwtHelperService.GetCompanyId();

            if (companyId == 0)
            {
                return Unauthorized(new { Status = "Error", Message = "Unauthorized: CompanyId missing in token." });
            }

            var plans = await _subscriptionRepository.GetAddonPlansAsync(companyId);

            return Ok(plans);
        }


        [HttpPost("renew")]
        public async Task<IActionResult> RenewSubscription([FromBody] RenewSubscriptionDto dto)
        {
            if (dto == null || dto.CompanyId <= 0 || dto.PlanId <= 0)
                return BadRequest("Invalid request");

            var userId = _jwtHelperService.GetUserId();

            if (userId == 0)
            {
                return Unauthorized(new { Status = "Error", Message = "Unauthorized: UserId missing in token." });
            }

            var result = await _subscriptionRepository.RenewSubscriptionAsync(userId, dto);

            if (result == null)
                return BadRequest("Plan not found or inactive");

            return Ok(new
            {
                success = true,
                transactionId = result.TransactionId,
                paymentLink = result.PaymentLink,
                paymentStatus = result.PaymentStatus,
                message = result.Message
            });
        }

        [HttpPost("renew/cancel")]
        public async Task<IActionResult> CancelPendingSubscription()
        {
            var companyId = _jwtHelperService.GetCompanyId();

            if (companyId == 0)
            {
                return Unauthorized(new { Status = "Error", Message = "Unauthorized: CompanyId missing in token." });
            }

            var userId = _jwtHelperService.GetUserId();

            if (userId == 0)
            {
                return Unauthorized(new { Status = "Error", Message = "Unauthorized: UserId missing in token." });
            }

            var result = await _subscriptionRepository.CancelPendingSubscriptionAsync(userId, companyId);

            if (result == null)
                return NotFound("Unable to process cancellation.");

            if (!result.Cancelled)
                return Ok(result);

            return Ok(result);
        }


        [HttpPost("renew/addon")]
        public async Task<IActionResult> AddAddonToSubscription(int userId, [FromBody] AddAddonDto dto)
        {
            var result = await _subscriptionRepository.AddAddonToSubscriptionAsync(userId, dto);

            if (result == null)
                return NotFound("Invalid addon plan(s) selected.");

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("mswipe/checkStatus")]
        public async Task<IActionResult> CheckTransactionStatus(string transId)
        {
            if (string.IsNullOrWhiteSpace(transId))
                return BadRequest("TransId is required.");

            try
            {
                var result = await _subscriptionRepository.CheckPaymentStatusAsync(transId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("expire-outdated")]
        public async Task<IActionResult> ExpireOutdatedSubscriptions()
        {
            var updatedCount = await _subscriptionRepository.ExpireOutdatedSubscriptionsAsync();

            return Ok(new
            {
                UpdatedCount = updatedCount,
                Message = updatedCount > 0
                    ? $"{updatedCount} subscription(s) expired."
                    : "No subscriptions needed expiry."
            });
        }
    }
}
