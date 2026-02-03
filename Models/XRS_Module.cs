using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Module", Schema = "dbo")]
    public class XRS_Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ModuleName { get; set; } = null!;

        [MaxLength(500)]
        public string? ModuleDescription { get; set; }

        public bool ModuleActive { get; set; } = true;
    }
}
