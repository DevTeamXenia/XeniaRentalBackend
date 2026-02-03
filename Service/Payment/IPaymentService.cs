
using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.Service.Payment
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLink(string orderId, decimal? netAmount);
        Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId);


    }
}
