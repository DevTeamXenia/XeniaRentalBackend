using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_SubscribePlan", Schema = "dbo")]
    public class XRS_SubscribePlan
    {
        [Key]
        [Column("planId")]
        public int PlanId { get; set; }

        [Column("planName")]
        [MaxLength(100)]
        public string? PlanName { get; set; }

        [Column("planDescription")]
        [MaxLength(100)]
        public string? PlanDescription { get; set; }

        [Column("planUsers")]
        public int PlanUsers { get; set; }

        [Column("planCreatedBy")]
        public int? PlanCreatedBy { get; set; }

        [Column("planCreatedOn")]
        public DateTime? PlanCreatedOn { get; set; }

        [Column("planModifiedBy")]
        public int? PlanModifiedBy { get; set; }

        [Column("planModifiedOn")]
        public DateTime? PlanModifiedOn { get; set; }

        [Column("planActive")]
        public bool? PlanActive { get; set; }

        [Column("PlanIsAddOn")]
        public bool PlanIsAddOn { get; set; }
    }
}