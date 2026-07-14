namespace XeniaRentalBackend.DTOs
{
    public class AddAddonResponseDto
    {
        public string? TransactionId { get; set; }
        public string? PaymentLink { get; set; }
        public string? PaymentStatus { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
