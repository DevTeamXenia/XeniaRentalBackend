using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Banners")]
    public class XRS_Banners
    {
        [Key]
        public int bannerID { get; set; }
        public required string bannerName { get; set; }
        public required string bannerImage { get; set; }
        public int? companyID { get; set; } 
        public int? propertyID { get; set; } 
        public bool bannerStatus { get; set; }
    }
}
