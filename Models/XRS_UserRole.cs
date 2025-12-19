using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{

    [Table("XRS_UserRole")]

    public class XRS_UserRole
    {

        [Key]
        public int UserRoleId { get; set; }


        public int CompanyId { get; set; }
      
        [Required]
        [StringLength(50)]
        public required string UserRoleName { get; set; }

      
        public bool isActive { get; set; }

 
        public DateTime CreatedDate { get; set; }

     
        public DateTime Modifieddate { get; set; }




    }
}
