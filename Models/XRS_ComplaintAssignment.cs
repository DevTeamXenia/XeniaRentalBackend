using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_ComplaintAssignment")]
    public class XRS_ComplaintAssignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public int CompanyId { get; set; }
        public int MaintenanceId { get; set; }

        [Required]
        [StringLength(50)]
        public string ComplaintNo { get; set; } = string.Empty;
        public int PropertyId { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public int CategoryId { get; set; }
        public int? UpdatedCategoryId { get; set; }
        public int? AssignedEmployeeId { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }

        [StringLength(500)]
        public string Complaint { get; set; } = string.Empty;

        [StringLength(50)]
        public string PreferredVisitTime { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        public string? Zone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}