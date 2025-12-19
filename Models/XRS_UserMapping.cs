using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.Models
{
    public class XRS_UserMapping
    {
        [Key]
        public int unitMapID { get; set; }
        public int userID { get; set; }
        public int propID { get; set; }
        public int unitID { get; set; }
        public bool isActive { get; set; }
    }

}
