using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_MessAttendance")]
    public class XRS_MessAttendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceID { get; set; }

        public int TenantID { get; set; }

        public int TenantAssignID { get; set; }

        public int BedSpacePlanID { get; set; }

        public int MessTypeID { get; set; }

        public DateTime AttendanceDate { get; set; }

        public bool IsPresent { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }
    }
}
