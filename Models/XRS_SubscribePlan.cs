using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_SubscribePlan", Schema = "dbo")]
    public class XRS_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required, MaxLength(200)]
        public string PlanName { get; set; } = null!;

        [MaxLength(500)]
        public string? PlanDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PlanPrice { get; set; }

        public int PlanDurationDays { get; set; }

        public bool PlanIsAddOn { get; set; } = true;

        public int? PlanCreatedBy { get; set; }

        public DateTime PlanCreatedOn { get; set; } = DateTime.Now;

        public int? PlanModifiedBy { get; set; }

        public DateTime? PlanModifiedOn { get; set; }

        public bool PlanActive { get; set; } = true;
    }
}
