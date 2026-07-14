
using XeniaRentalBackend.Dtos;
using Stripe.Checkout;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Payment;

namespace XeniaRentalBackend.Service.Payment
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLink(string orderId, decimal? netAmount);
        Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId);
        Task<string> CreateSubPaymentLink(string orderId, decimal? netAmount);
        Task<MswipeTransactionStatusResponse> CheckSubTransactionStatusAsync(string transId);
        Task<string> CreateOrderAsync(decimal amount, string currency, string apiKey, string apiSecret, string receiptNo, string customerName, string mobileNumber);
        Task<string> GetOrderStatusAsync(string orderId, string apiKey, string apiSecret);
    }
}
