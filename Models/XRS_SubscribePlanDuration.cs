using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models.Rental
{
    [Table("XRS_SubscribePlanDuration")]
    public class XRS_SubscribePlanDuration
    {
        [Key]
        [Column("planDurationId")]
        public int PlanDurationId { get; set; }

        [Required]
        [Column("planId")]
        public int PlanId { get; set; }

        [Required]
        [Column("durationDays")]
        public int DurationDays { get; set; }   // 30, 90, 365

        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("createdBy")]
        public int? CreatedBy { get; set; }

        [Column("createdOn")]
        public DateTime CreatedOn { get; set; }

        [Column("modifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        // 🔹 Navigation
        [ForeignKey(nameof(PlanId))]
        public XRS_SubscribePlan Plan { get; set; } = null!;
    }
}
