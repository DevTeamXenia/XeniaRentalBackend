namespace XeniaRentalBackend.Dtos
{
    public class CompanyWithSubscriptionDto
    {
        public CompanyDto Company { get; set; } = new();
        public SubscriptionDto? Subscription { get; set; }
        public PlanDto? Plan { get; set; }
    }

    public class SubscriptionDto
    {
        public int SubId { get; set; }
        public int PlanId { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public int SubscriptionDays { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class SubscriptionAddonDto
    {
        public int SubAddonId { get; set; }
        public decimal Amount { get; set; }
        public int UserCount { get; set; }
    }
}
