using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XeniaRentalBackend.Models.Rental;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_SubscribePlan", Schema = "dbo")]
    public class XRS_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }

        [MaxLength(500)]
        public string PlanName { get; set; }

        [MaxLength(4000)]
        public string? PlanDescription { get; set; }

        public int PlanUsers { get; set; }
        public decimal? PlanPrice { get; set; }
        public decimal? PlanDPrice { get; set; }
        public decimal? PlanCPrice { get; set; }

        public bool PlanIsAddOn { get; set; }

        public bool PlanActive { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public DateTime? ModifiedOn { get; set; }
        public virtual ICollection<XRS_SubscribePlanDuration> PlanDurations { get; set; }
            = new List<XRS_SubscribePlanDuration>();
    }
}
