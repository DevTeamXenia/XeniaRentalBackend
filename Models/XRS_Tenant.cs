using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Tenant")]
    public class XRS_Tenant
    {
        [Key]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonInclude]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tenantID { get; set; }
        public int companyID { get; set; }
        public required string tenantName { get; set; }
        public required string phoneNumber { get; set; }
        public string email { get; set; }
        public string? deviceToken { get; set; }
        public string? emergencyContactNo { get; set; }
        public decimal concessionper { get; set; }
        public string? note { get; set; }
        public string? address { get; set; }
        public bool isActive { get; set; }
 
        [JsonIgnore] 
        public virtual ICollection<XRS_TenantDocuments>? TenantDocuments { get; set; }

        [JsonIgnore]
        public virtual ICollection<XRS_TenantChequeRegister>? TenantChequeRegister { get; set; }
    }
}
