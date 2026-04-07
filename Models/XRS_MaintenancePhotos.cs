using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_MaintenancePhotos")]
    public class XRS_MaintenancePhotos
    {
        [Key]
        public int PhotoId { get; set; }

        public int MaintenanceId { get; set; }

        [Required]
        public string PhotoUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("MaintenanceId")]
        public XRS_Maintenance Maintenance { get; set; } = null!;
    }
}