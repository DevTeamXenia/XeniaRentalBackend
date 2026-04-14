using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Area")]
    public class XRS_Area
    {
        [Key]
        public int AreaId { get; set; }

        public int CompanyID { get; set; }

        public  required string AreaName { get; set; }

        public required string AreaCode { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public ICollection<XRS_PropertyAreas> PropertyAreas { get; set; }
    }
}