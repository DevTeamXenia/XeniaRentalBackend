
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Users")]

    public class XRS_Users
    {
        [Key]
        [Column("userID")]
        public int UserId { get; set; }

        [Column("companyID")]
        public int CompanyId { get; set; }

        [Column("userType")]
        public int UserType { get; set; }

        [Column("userName")]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Column("password")]
        [Required]
        [StringLength(50)]
        public required string Password { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("modifiedDate")]
        public DateTime? Modifieddate { get; set; }

        [NotMapped]
        public string? UsetTypeName { get; set; }

        [ForeignKey("UserType")]
        [JsonIgnore]
        public XRS_UserRole? UserRole { get; set; }




    }
}
