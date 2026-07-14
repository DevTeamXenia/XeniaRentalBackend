namespace XeniaRentalBackend.Dtos
{
    public class RenewSubscriptionResponseDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentLink { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
    public class RenewSubscriptionDto
    {
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public List<int>? AddonPlanIds { get; set; }
    }
}
