using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_VoucherDetails")]
    public class XRS_VoucherDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int voucherDetailId { get; set; }

        [ForeignKey("Voucher")]
        public int voucherId { get; set; }

        [ForeignKey("Charge")]
        public int chargeId { get; set; }

        public decimal amount { get; set; }

        public virtual XRS_Voucher Voucher { get; set; }
        public virtual XRS_Charges Charge { get; set; }
    }
}
