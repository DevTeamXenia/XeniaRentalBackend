using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models.Rental
{
    [Table("XRS_SubscribePlanDuration")]
    public class XRS_SubscribePlanDuration
    {
        [Key]
        public int PlanDurationId { get; set; }

        public int? PlanId { get; set; }

        public int DurationDays { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DealerPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomPrice { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey(nameof(PlanId))]
        public virtual XRS_SubscribePlan? SubscribePlan { get; set; }
    }
}
