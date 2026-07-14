using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaTenoraBackend.Models
{
    [Table("XRS_CompanySubscriptionSpecialRate", Schema = "dbo")]
    public class XRS_CompanySubscriptionSpecialRate
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public int? PlanDurationId { get; set; }
        public int? AddonId { get; set; }
        public decimal CustomRate { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
