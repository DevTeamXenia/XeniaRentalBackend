using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Properties")]
    public class XRS_Properties
    {
        [Key]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonInclude]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PropID { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        public required string propertyName { get; set; }
     
        public string propertyType { get; set; }

        public bool IsActive { get; set; }


    }
}
