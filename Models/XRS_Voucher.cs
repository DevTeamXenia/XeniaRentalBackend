using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{

    [Table("XRS_Voucher")]
    public class XRS_Voucher
    {

        [Key]
        public int VoucherID { get; set; }

        public int unitID {  get; set; }

        public int  CompanyID { get; set; }

        public int PropID { get; set; }

        public string? VoucherNo { get; set; }

        public DateTime VoucherDate { get; set; }

        public string? VoucherType { get; set; }

        public int DrID { get; set; }

        public int CrID { get; set; }

        public decimal Amount { get; set; }

        public string? RefNo { get; set; }

        public string? Remarks { get; set; }

        public string? IssueingBank { get; set; }

        public string? ChequeNo { get; set; }

        public bool Cancelled { get; set; }

        public decimal CrAmount { get; set; }

        public bool IsReconcil { get; set; }

        public bool? ChequeStatus { get; set; }

        public DateTime? ReconcilDate { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModificationBy { get; set; }
        public string? VoucherStatus { get; set; }

        public bool isActive {  get; set; }

    }
}
