using Stripe.Checkout;
//using XeniaRentalBackend.Repositories.Order;

namespace XeniaRentalBackend.Service.Payment
{
    public interface IPaymentService
    {
        Task<string> CreateOrderAsync(decimal amount, string currency, string apiKey, string apiSecret, string receiptNo);
        Task<string> GetOrderStatusAsync(string orderId, string apiKey, string apiSecret);
        //Task<PaymentStatusResult> GetLastPaymentStatusAsync(string orderId, string apiKey, string apiSecret);
        Task<Session> CreateCheckoutSession(string productName, long amount, string currency, string successUrl, string cancelUrl);


    }
}
