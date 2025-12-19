using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Accounts")]
    public class XRS_Accounts
    {
        [Key]
        public int accID { get; set; }

        public int companyID { get; set; }

        public int VoucherId { get; set; }
        public int GroupId { get; set; }

        public string invType { get; set; }

        public string invNo { get; set; }

        public DateTime invDate { get; set; }

        public int ledgerDr { get; set; }

        public int ledgerCr { get; set; }

        public decimal amountDr { get; set; }

        public decimal amountCr { get; set; }

        public string remarks { get; set; }

        public DateTime createdOn { get; set; }

        public DateTime modifiedOn { get; set; }

        public string createdBy { get; set; }

        public string modifiedBy { get; set; }

        public bool isActive { get;set; }   
    }
}
