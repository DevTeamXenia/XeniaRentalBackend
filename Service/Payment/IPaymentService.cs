
using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.Service.Payment
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLink(string orderId, decimal? netAmount);
        Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId);
        Task<string> CreateOrderAsync(decimal amount, string currency, string apiKey, string apiSecret, string receiptNo, string customerName, string mobileNumber);
        Task<string> GetOrderStatusAsync(string orderId, string apiKey, string apiSecret);

    }
}
