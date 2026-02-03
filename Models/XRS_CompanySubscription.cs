using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.Models
{
    public class XRS_CompanySubscription
    {
        [Key]
        public int SubId { get; set; }

        public int PlanId { get; set; }

        public int CompanyId { get; set; }

        public DateTime SubscriptionDate { get; set; } = DateTime.Now;

        public DateTime SubscriptionStartDate { get; set; }

        public DateTime SubscriptionEndDate { get; set; }

        public decimal SubscriptionAmount { get; set; }


        public int SubscriptionDays { get; set; }


        [MaxLength(50)]
        public string Status { get; set; } = "ACTIVE";
    }
}
