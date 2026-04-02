using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_MaintenanceCategory")]
    public class XRS_MaintenanceCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public int CompanyId { get; set; }

        public string CategoryName { get; set; }

        public int SLADays { get; set; }

        public int SLAHours { get; set; }

        public bool IsActive { get; set; }
    }
}
