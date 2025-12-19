using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_BedspaceAssignment")]
    public class BedSpaceAssignemnt
    {
        [Key]
        public int assignmentID {  get; set; }

        [ForeignKey(nameof(Property))]
        public int propID { get; set; }

        [ForeignKey(nameof(Units))]
        public int unitID { get; set; }

        [ForeignKey(nameof(Tenant))]
        public int tenantID { get; set; }

        [ForeignKey(nameof(BedSpace))]
        public int bedSpaceID { get; set; }

        public int companyID { get;set; }

        public decimal rentAmt { get; set; }

        public bool isTaxable { get; set; }

        public string taxRatePer { get; set; }

        public string rentConcession { get; set; }

        public string messConcession { get; set; }  

        public DateTime agreementStartDate { get; set; }

        public DateTime agreementEndDate { get; set; }

        public string frequency { get; set; }

        public DateTime rentDueDate { get; set; }

        public decimal rentescalationPer { get; set; }

        public DateTime escalationDate { get; set; }

        public decimal securityAmt { get; set; }

        public decimal charges { get; set; }

        public decimal amount { get; set; }

        public bool isActive { get; set; }

        [NotMapped]
        public string? PropName { get; set; }

        [NotMapped]
        public string? UnitName { get; set; }

        [NotMapped]
        public string? TenantName { get; set; }

        [NotMapped]
        public string? BedSpaceName { get; set; }

        [JsonIgnore]
        public virtual XRS_Properties? Property { get; set; }


        [JsonIgnore]
        public virtual XRS_Units? Units { get; set; }

        [JsonIgnore]
        public virtual XRS_Tenant? Tenant { get; set; }

        [JsonIgnore]
        public virtual XRS_Bedspace? BedSpace { get; set; }

    }
}
