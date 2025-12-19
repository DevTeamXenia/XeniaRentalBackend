using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.DTOs
{
    public class CreateUser
    {
        public int CompanyId { get; set; }

        /// <summary>
        /// UserType for storing user type like admin,tenant,landonwer
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// Required
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        /// <summary>
        /// Required
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Is Active
        /// Required
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Created Date
        /// Optional
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Modified date
        /// Optional
        /// </summary>
        public DateTime Modifieddate { get; set; }
    }
}
