using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Messtypes")]
    public class XRS_Messtypes
    {
        [Key]
        public int messID { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        public required string MessName { get; set; }
        public required string MessCode { get; set; }

        public bool IsActive { get; set; }
    }
}
