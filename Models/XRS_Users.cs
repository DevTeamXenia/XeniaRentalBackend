
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Users")]

    public class XRS_Users
    {

        [Key]
        public int UserId { get; set; }

        public int CompanyId { get; set; }
        
        public int UserType { get; set; }

        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public required string Password { get; set; }

        public  string Phone { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? Modifieddate { get; set; }

        [NotMapped]
        public string UsetTypeName { get; set; }

        [ForeignKey("UserType")]
        [JsonIgnore]
        public XRS_UserRole? UserRole { get; set; }




    }
}
