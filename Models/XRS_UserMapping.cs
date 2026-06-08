using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_UserMapping")]
    public class XRS_UserMapping
    {
        [Key]
        public int UnitMapID { get; set; }

        public int UserID { get; set; }

        public int PropID { get; set; }

        public bool IsActive { get; set; }

        [JsonIgnore]
        [ForeignKey("UserID")]
        public XRS_Users? User { get; set; }

        [JsonIgnore]
        [ForeignKey("PropID")]
        public XRS_Properties? Property { get; set; }

        [NotMapped]
        public string? PropertyName { get; set; }
    }
}