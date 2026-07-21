using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_CompanySubscription")]
    public class XRS_CompanySubscription
    {
        [Key]
        public int SubId { get; set; }

        public int? PlanId { get; set; }

        public int? PlanDurationId { get; set; }

        public int? CompanyId { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        public int? SubscriptionDays { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SubscriptionAmount { get; set; }

        [MaxLength(50)]
        public string? RateType { get; set; }

        public int? SubscriptionUserCount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "ACTIVE";

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

    }
}

