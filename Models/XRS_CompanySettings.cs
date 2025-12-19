using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_CompanySettings")]
    public class XRS_CompanySettings
    {
        [Key]
        public int Id { get; set; }

        public int companyID { get; set; }

        public string? emailIincomingServer { get; set; }

        public string? emailSender { get; set; }

        public string? password { get; set; }

        public bool userDefaultCredentials { get; set; }

        public string? port { get; set; }

        public bool? enableSsl { get; set; }

        public string? host { get; set; }
        public string? smsGateWay { get; set; }
        public string? termandcondition { get; set; }
        public string? privacyPolicy { get; set; }
        public string? help { get; set; }

        public bool active { get; set; }
    }
}
