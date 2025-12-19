using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{

    [Table("XRS_TenantDocuments")]
    public class XRS_TenantDocuments
    {
 
        [Key]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonInclude]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TenantDocId { get; set; }

   
        public int DocTypeId { get; set; }

        public int CompanyID { get; set; }

        public int TenantID { get; set; }

        public string DocumentsNo{ get; set; }
	
        public string Documenturl  {get;set;}

        public bool isActive { get; set; }

        [ForeignKey("TenantID")]
        [JsonIgnore]
        public virtual XRS_Tenant? Tenant { get; set; }

        [ForeignKey("DocTypeId")]
        [JsonIgnore]
        public virtual XRS_Documents? Documents { get; set; }

        [NotMapped]
        public string? DocumentName { get; set; }

        [NotMapped]
        public bool? IsAlphaNumeric { get; set; }

        [NotMapped]
        public bool? IsExpiry { get; set; }

        [NotMapped]
        public bool? IsMandatory { get; set; }

        [NotMapped]
        public DateTime? ExpiryDate { get; set; }

        [NotMapped]
        public string? DocPurpose { get; set; }

    }
}
