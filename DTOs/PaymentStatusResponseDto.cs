namespace XeniaRentalBackend.DTOs
{
    public class PaymentStatusResponseDto
    {
        public string? TransactionId { get; set; }
        public string? SubscriptionStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
