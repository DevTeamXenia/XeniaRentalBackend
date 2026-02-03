using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_CompanySettings")]
    public class XRS_CompanySettings
    {
        [Key]
        public int CompanySettingsId { get; set; }

        public int CompanyId { get; set; }

        [Required, MaxLength(100)]
        public string KeyCode { get; set; } = null!;

        [Required]
        public string Value { get; set; } = null!;

        public bool Active { get; set; } = true;
    }
}
