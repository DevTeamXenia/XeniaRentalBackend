using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Documents")]
    public class XRS_Documents
    {
        [Key]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonInclude]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int docTypeId { get; set; }

        public int companyID { get; set; }

        public string  docName { get; set; }

        public string docPurpose { get; set; }

        public bool isAlphanumeric { get; set; }

        public bool isExpiry { get; set; }

        public bool isMandatory { get; set; }

        public bool isActive { get; set; }

        public DateTime? ExpiryDate { get; set; }

    }

}
