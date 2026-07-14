namespace XeniaRentalBackend.DTOs
{
    public class CancelSubscriptionResponseDto
    {
        public bool Cancelled { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
