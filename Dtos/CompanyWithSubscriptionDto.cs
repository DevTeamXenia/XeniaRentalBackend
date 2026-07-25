using XeniaTenoraBackend.DTOs;

namespace XeniaRentalBackend.Dtos
{
     public class CompanyWithSubscriptionDto
    {
            public int TotalUser { get; set; }
            public CompanyDto Company { get; set; } = new();
            public SubscriptionDto? Subscription { get; set; }
            public PlanDto? Plan { get; set; }
            public List<SubscriptionAddonDto> Addons { get; set; } = new();
            public List<CompanySettingDto> CompanySettings { get; set; } = new();
    }

        public class SubscriptionDto
        {
            public int SubId { get; set; }
            public int? PlanId { get; set; }
            public DateTime? SubscriptionStartDate { get; set; }
            public DateTime? SubscriptionEndDate { get; set; }
            public decimal? SubscriptionAmount { get; set; }
            public int? SubscriptionDays { get; set; }
            public int? SubscriptionUserCount { get; set; }
            public string? RateType { get; set; }
            public int? ExpireDays { get; set; }
            public string? Status { get; set; } = string.Empty;
        }

        public class SubscriptionAddonDto
        {
            public int Id { get; set; }
            public int? PlanId { get; set; }
            public string? PlanName { get; set; }
            public decimal? Amount { get; set; }
            public decimal? DealerAmount { get; set; }
            public string? RateType { get; set; }
            public int? UserCount { get; set; }
            public string? Status { get; set; }
        }

        public class CompanySettingDto
        {
            public int Id { get; set; }
            public string? KeyCode { get; set; }
            public string? Value { get; set; }
        }
}
