using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XeniaRentalBackend.Models;
namespace XeniaTenoraBackend.Models
{
    [Table("XRS_EmployeeArea")]
    public class XRS_EmployeeArea
    {
        [Key]
        public int EmployeeAreaId { get; set; }
        public int EmployeeId { get; set; }
        public int AreaId { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public XRS_Employee XRS_Employee { get; set; }
    }
}
