using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Maintenance")]
    public class XRS_Maintenance
    {
        [Key]
        public int MaintenanceId { get; set; }

        public int CompanyId { get; set; }

        public int? TenantId  { get; set; }

        [Required]
        [StringLength(50)]
        public string ComplaintNo { get; set; } = string.Empty;

        public int PropertyId { get; set; }

        [Required]
        public int UnitId { get; set; } 

        public int CategoryId { get; set; }

        [Required]
        [StringLength(500)]
        public string Complaint { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PreferredVisitTime { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public int? AssignedEmployeeId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Property
        public List<XRS_MaintenancePhotos> Photos { get; set; } = new();
    }
}