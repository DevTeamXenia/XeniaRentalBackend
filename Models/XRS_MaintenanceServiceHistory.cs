using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_MaintenanceServiceHistory")]
    public class XRS_MaintenanceServiceHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public int ServiceId { get; set; }
        public int MaintenanceId { get; set; }
        public int CompanyId { get; set; }

        [StringLength(500)]
        public string? Complaint { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string Report { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }


}