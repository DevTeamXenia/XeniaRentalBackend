using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Service")]
    public class XRS_Service
    {
        [Key]
        public int serviceID { get; set; }
        public required string serviceName { get; set; }
        public required string servicePhoneNumber { get; set; }
        public required string serviceWhatappNumber { get; set; }
        public int serviceCompanyID { get; set; }
        public int servicePropertyID { get; set; }
        public string serviceType { get; set; }
        public bool ServiceStatus { get; set; }
    }
}
