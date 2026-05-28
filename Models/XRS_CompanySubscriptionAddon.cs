using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace XeniaTenoraBackend.Models
{

    [Table("XRS_CompanySubscriptionAddon")]
    public class XRS_CompanySubscriptionAddon
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public decimal Amount { get; set; }
        public int DepCount { get; set; }
        public string? Status { get; set; }
        public int MainPlanId { get; set; }
    }
}
