using System.Text.Json.Serialization;

namespace XeniaTenoraBackend.DTOs
{
    public class MswipePaymentRequest
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; }  // ✅ string, not decimal

        [JsonPropertyName("mobileno")]
        public string MobileNo { get; set; }

        [JsonPropertyName("custcode")]
        public string CustCode { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("sessiontoken")]
        public string SessionToken { get; set; }

        [JsonPropertyName("versionno")]
        public string VersionNo { get; set; }

        [JsonPropertyName("email_id")]
        public string EmailId { get; set; }

        [JsonPropertyName("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        [JsonPropertyName("ApplicationId")]
        public string ApplicationId { get; set; }

        [JsonPropertyName("ChannelId")]
        public string ChannelId { get; set; }

        [JsonPropertyName("ClientId")]
        public string ClientId { get; set; }
    }
}


