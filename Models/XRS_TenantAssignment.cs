using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_TenantAssignment")]
    public class XRS_TenantAssignment
    {
        [Key]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonInclude]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tenantAssignId { get; set; }

        public int propID { get; set; }
        public int unitID { get; set; }
        public int tenantID { get; set; }
        public int companyID { get; set; }
        public decimal securityAmt { get; set; }
        public bool isTaxable { get; set; }
        public int taxPercentage { get; set; }
        public int? bedSpaceID { get; set; }
        public decimal rentAmt { get; set; }
        public int rentConcession { get; set; }
        public int messConcession { get; set; }
        public string collectionType { get; set; }
        public DateTime agreementStartDate { get; set; }
        public DateTime agreementEndDate { get; set; }
        public int rentCollection { get; set; }
        public int escalationPer { get; set; }
        public DateTime? nextescalationDate { get; set; }
        public DateTime rentDueDate { get; set; }
        public string? closureReason { get; set; }
        public DateTime? closureDate { get; set; }
        public decimal refundAmount { get; set; }
        public decimal charges { get; set; }
        public decimal amount { get; set; }
        public string? paymentMode { get; set; }
        public string? notes { get; set; }
        public bool isClosure { get; set; }
        public bool isActive { get; set; }

        [NotMapped] public string? PropName { get; set; }
        [NotMapped] public string? UnitName { get; set; }
        [NotMapped] public string? TenantName { get; set; }
        [NotMapped] public string? TenantContactNo { get; set; }
        [NotMapped] public string? BedSpaceName { get; set; }

        [ForeignKey("propID")]
        [JsonIgnore]
        public virtual XRS_Properties? Properties { get; set; }

        [ForeignKey("tenantID")]
        [JsonIgnore]
        public virtual XRS_Tenant? Tenant { get; set; }

        [ForeignKey("unitID")]
        [JsonIgnore]
        public virtual XRS_Units? Unit { get; set; }

        [ForeignKey("bedSpaceID")]
        [JsonIgnore]
        public virtual XRS_Bedspace? BedSpace { get; set; }

    }
}
