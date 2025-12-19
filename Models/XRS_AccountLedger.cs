using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_AccountLedger")]
    public class XRS_AccountLedger
    {
        [Key]
        public int ledgerID { get; set; }

        public int groupID { get; set; }

        public int companyID { get; set; }

        public string ledgerCode { get; set; }

        public string ledgerName { get; set; }

        public DateTime createdOn { get; set; }

        public string createdBy { get; set; }

        public DateTime modifiedOn { get; set; }

        public string modifiedBy { get; set; }

        public bool isActive { get; set; }

    }
}
