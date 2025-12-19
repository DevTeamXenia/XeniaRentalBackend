using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Stripe.Checkout;
//using XeniaRentalBackend.Repositories.Order;

namespace XeniaRentalBackend.Service.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateOrderAsync(
            decimal amount,
            string currency,
            string apiKey,
            string apiSecret,
            string receiptNo)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
          
            var byteArray = Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var orderData = new
            {
                amount = (int)(amount * 100), 
                currency,
                receipt = receiptNo,
                payment_capture = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(orderData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Razorpay Error ({response.StatusCode}): {errorResponse}");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic order = JsonConvert.DeserializeObject(json);

            return order.id;
        }


        public async Task<string> GetOrderStatusAsync(string razorpayOrderId, string apiKey, string apiSecret)
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.GetAsync($"https://api.razorpay.com/v1/orders/{razorpayOrderId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<dynamic>(json);
                string status = obj["status"];

                return status;
            }
        }

        //public async Task<PaymentStatusResult> GetLastPaymentStatusAsync(string orderId, string apiKey, string apiSecret)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get,
        //        $"https://api.razorpay.com/v1/orders/{orderId}/payments");

        //    var byteArray = Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}");
        //    request.Headers.Authorization =
        //        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        //    var response = await _httpClient.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var json = await response.Content.ReadAsStringAsync();
        //    using var doc = JsonDocument.Parse(json);

        //    var root = doc.RootElement;
        //    var result = new PaymentStatusResult();

        //    if (!root.TryGetProperty("items", out var items) ||
        //        items.ValueKind != JsonValueKind.Array ||
        //        items.GetArrayLength() == 0)
        //    {
        //        result.Status = "created";
        //        return result;
        //    }


        //    var last = items[items.GetArrayLength() - 1];

        //    if (last.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
        //        result.PaymentId = idProp.GetString()!;
       
        //    if (last.TryGetProperty("status", out var statusProp) && statusProp.ValueKind == JsonValueKind.String)
        //        result.Status = statusProp.GetString()!;

        //    if (last.TryGetProperty("method", out var methodProp) && methodProp.ValueKind == JsonValueKind.String)
        //        result.Method = methodProp.GetString()!;

        //    return result;
        //}

        public async Task<Session> CreateCheckoutSession(string productName, long amount, string currency, string successUrl, string cancelUrl)
        {
            if (currency.ToLower() == "inr" && amount < 5200)
            {
                amount = 5200;
            }
            else if (currency.ToLower() == "eur")
            {
                amount = amount * 100;

            }


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = amount,
                    Currency = currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = productName
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            return service.Create(options);
        }


    }
}
