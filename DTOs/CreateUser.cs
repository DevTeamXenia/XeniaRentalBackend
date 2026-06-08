using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.DTOs
{
    public class CreateUser
    {
        public int CompanyId { get; set; }

        public int UserType { get; set; }

        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public required string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime Modifieddate { get; set; }
        public List<UserMappingDto>? UserMappings { get; set; }
    }

    public class UserMappingDto
    {
        public int PropID { get; set; }

        public bool IsActive { get; set; }
    }
}
