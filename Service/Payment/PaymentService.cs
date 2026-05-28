using System.Text;
using System.Text.Json;
using XeniaRentalBackend.Dtos;
using XeniaTenoraBackend.DTOs;


namespace XeniaRentalBackend.Service.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;

        /*  private const string MSWIPE_USER_ID = "9072454466";
          private const string MSWIPE_CLIENT_ID = "MSW*PBLMOH9401987199";
          private const string MSWIPE_CLIENT_SECRET = "ecd55fb5fe1e9329a7e03d7dc1575217e07e726eea4bceaacb4c5001fd8a511d";
          private const string MERCHANT_CODE = "9401987199";*/


        private const string MSWIPE_USER_ID = "9539484666";
        private const string MSWIPE_CLIENT_ID = "MSW*PBLAru9401004474";
        private const string MSWIPE_CLIENT_SECRET = "23f294d62327afca90b0a89076d82c9b3d0a499759819dc58cf7277f2ff2757b";
        private const string MERCHANT_CODE = "9401004474";

        private const string UAT_AUTH_URL = "https://dcuat.mswipetech.co.in/ipg/api/CreatePBLAuthToken";
        private const string UAT_PAYMENT_URL = "https://dcuat.mswipetech.co.in/ipg/api/MswipePayment";

        private const string PROD_AUTH_URL = "https://pbl.mswipe.com/ipg/api/CreatePBLAuthToken";
        private const string PROD_PAYMENT_URL = "https://pbl.mswipe.com/ipg/api/MswipePayment";

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreatePaymentLink(string orderId, decimal? netAmount)
        {
            string token = await GenerateAuthToken();
            return await GeneratePaymentLink(orderId, netAmount, token);
        }


        private async Task<string> GenerateAuthToken()
        {
            var tokenRequest = new
            {
                userId = MSWIPE_USER_ID,
                clientId = MSWIPE_CLIENT_ID,
                password = MSWIPE_CLIENT_SECRET,
                applId = "api",
                channelId = "pbl"
            };

            var tokenResponse = await _httpClient.PostAsJsonAsync(
             UAT_AUTH_URL,
              tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
                throw new Exception("Failed to generate MSWIPE token.");

            var tokenResult = await tokenResponse.Content.ReadFromJsonAsync<MswipeTokenResponse>();

            if (tokenResult == null || string.IsNullOrEmpty(tokenResult.token))
                throw new Exception("MSWIPE token generation failed: " + tokenResult?.msg);

            return tokenResult.token;
        }

        //private async Task<string> GeneratePaymentLink(string orderId, decimal? netAmount, string token)
        //{
        //    var json = $@"
        //    {{
        //      ""amount"": ""{netAmount:F2}"",
        //      ""mobileno"": ""9999999999"",
        //      ""custcode"": ""{MERCHANT_CODE}"",
        //      ""user_id"": ""{MSWIPE_USER_ID}"",
        //      ""sessiontoken"": ""{token}"",
        //      ""versionno"": ""VER4.0.0"",
        //      ""email_id"": ""customer@test.com"",
        //      ""invoice_id"": ""{orderId}"",
        //      ""request_id"": ""{Guid.NewGuid():N}"",
        //      ""ApplicationId"": ""api"",
        //      ""ChannelId"": ""pbl"",
        //      ""ClientId"": ""{MSWIPE_CLIENT_ID}""
        //    }}";

        //    using var content = new StringContent(
        //        json,
        //        Encoding.UTF8,
        //        "application/json");

        //    var response = await _httpClient.PostAsync(
        //        UAT_PAYMENT_URL,
        //        content);

        //    var rawJson = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //        throw new Exception($"HTTP ERROR: {rawJson}");

        //    var result = JsonSerializer.Deserialize<MswipePaymentResponse>(
        //        rawJson,
        //        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    if (result == null || result.status != "True")
        //        throw new Exception($"MSWIPE ERROR: {result?.responsemessage}");

        //    return result.smslink;
        //}

        private async Task<string> GeneratePaymentLink(string orderId, decimal? netAmount, string token)
        {
            if (!netAmount.HasValue)
                throw new Exception("Amount is null");

            var request = new MswipePaymentRequest
            {
                Amount = netAmount.Value.ToString("F2"),
                MobileNo = "9999999999",
                CustCode = MERCHANT_CODE,
                UserId = MSWIPE_USER_ID,
                SessionToken = token,
                VersionNo = "VER4.0.0",
                EmailId = "customer@test.com",
                InvoiceId = orderId,
                RequestId = Guid.NewGuid().ToString("N"),
                ApplicationId = "api",
                ChannelId = "pbl",
                ClientId = MSWIPE_CLIENT_ID
            };

            var response = await _httpClient.PostAsJsonAsync(UAT_PAYMENT_URL, request);

            var rawJson = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"MSWIPE FAILED: HTTP {(int)response.StatusCode} | RESPONSE: {rawJson}");
            }
            var result = JsonSerializer.Deserialize<MswipePaymentResponse>(
                rawJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null || result.status != "True")
                throw new Exception($"MSWIPE ERROR: {result?.responsemessage} | RAW: {rawJson}");

            return result.smslink;
        }


        //public async Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId)
        //{

        //    var statusRequest = new { id = transId };

        //    var statusResponse = await _httpClient.PostAsJsonAsync(
        //        "https://pbl.mswipe.com/ipg/api/getPBLTransactionDetails",
        //        statusRequest);

        //    var rawJson = await statusResponse.Content.ReadAsStringAsync();

        //    if (!statusResponse.IsSuccessStatusCode)
        //        throw new Exception($"Transaction status failed. Raw: {rawJson}");

        //    var result = JsonSerializer.Deserialize<MswipeTransactionStatusResponse>(
        //        rawJson,
        //        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    if (result == null || !string.Equals(result.Status, "True", StringComparison.OrdinalIgnoreCase))
        //        throw new Exception("Transaction status check failed: " + result?.ResponseMessage);

        //    return result;
        //}


        public async Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId)
        {
            // ✅ Extract MSwipe TransID from payment link URL if needed
            if (transId.Contains("TransID="))
                transId = transId.Split("TransID=")[1];

            // ✅ Get auth token
            string token = await GenerateAuthToken();

            var statusRequest = new
            {
                id = transId,
                sessiontoken = token,        // ✅ auth
                user_id = MSWIPE_USER_ID,    // ✅ auth
                ClientId = MSWIPE_CLIENT_ID, // ✅ auth
                ApplicationId = "api",
                ChannelId = "pbl"
            };

            // ✅ UAT URL not PROD
            var statusResponse = await _httpClient.PostAsJsonAsync(
                "https://dcuat.mswipetech.co.in/ipg/api/getPBLTransactionDetails",
                statusRequest);

            var rawJson = await statusResponse.Content.ReadAsStringAsync();
            Console.WriteLine("RAW STATUS RESPONSE: " + rawJson); // ✅ check terminal

            if (!statusResponse.IsSuccessStatusCode)
                throw new Exception($"Transaction status failed. Raw: {rawJson}");

            var result = JsonSerializer.Deserialize<MswipeTransactionStatusResponse>(
                rawJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null || !string.Equals(result.Status, "True", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Transaction status check failed: " + result?.ResponseMessage);

            return result;
        }
    }

}

