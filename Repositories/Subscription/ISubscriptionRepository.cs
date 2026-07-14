using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;

namespace XeniaRentalBackend.Repositories.Subscription
{
    public interface ISubscriptionRepository
    {
        Task<List<PlanWithModulesDto>> GetMainPlansAsync(int companyId);
        Task<List<AddonPlanDto>> GetAddonPlansAsync(int companyId);
        Task<RenewSubscriptionResponseDto?> RenewSubscriptionAsync(int customerId, RenewSubscriptionDto dto);
        Task<CancelSubscriptionResponseDto?> CancelPendingSubscriptionAsync(int userId, int companyId);
        Task<AddAddonResponseDto?> AddAddonToSubscriptionAsync(int userId, AddAddonDto dto);
        Task<PaymentStatusResponseDto?> CheckPaymentStatusAsync(string transactionId);
        Task<int> ExpireOutdatedSubscriptionsAsync();

    }
}
