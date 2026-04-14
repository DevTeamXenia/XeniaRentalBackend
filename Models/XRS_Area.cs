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

        [Required]
        public string AreaName { get; set; }

        [Required]
        public string AreaCode { get; set; }

        public bool Active { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; }


    }
}