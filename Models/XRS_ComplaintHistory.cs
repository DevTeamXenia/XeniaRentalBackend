using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_ComplaintHistory")]
    public class XRS_ComplaintHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int MaintenanceId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Report { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

       
    }
}