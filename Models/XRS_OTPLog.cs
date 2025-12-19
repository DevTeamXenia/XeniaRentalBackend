using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_OTPLog")]
    public class XRS_OTPLog
    {
        [Key]
        public int OTPId { get; set; }

        public int CompanyId { get; set; }

        public required string MobileNo { get; set; }

        public int Type { get; set; }

        public required string OTP { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
