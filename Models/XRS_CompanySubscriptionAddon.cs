using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace XeniaTenoraBackend.Models
{

    [Table("XRS_CompanySubscriptionAddon")]
    public class XRS_CompanySubscriptionAddon
    {
        [Key]
        public int Id { get; set; }

        public int? MainPlanId { get; set; }

        public int? PlanId { get; set; }

        public int? CompanyId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DealerAmount { get; set; }

        [MaxLength(50)]
        public string? RateType { get; set; }

        public int? UserCount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "ACTIVE";

        public DateTime? CreatedOn { get; set; } = DateTime.Now;
    }
}
