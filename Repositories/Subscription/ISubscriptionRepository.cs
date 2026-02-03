using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.Repositories.Subscription
{
    public interface ISubscriptionRepository
    {
        Task<List<PlanWithModulesDto>> GetMainPlansAsync();
        Task<RenewSubscriptionResponseDto?> RenewSubscriptionAsync(RenewSubscriptionDto dto);
        Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId);
        Task<bool> UpdatePaymentStatusAsync(string transactionRefId, bool isSuccess);
    }
}
