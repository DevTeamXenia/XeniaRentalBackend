using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace XeniaRentalBackend.Models
{
    [Table("XRS_Charges")]
    public class XRS_Charges
    {
        [Key]
        public int chargeID { get; set; }

        public int PropID { get; set; }

        public int companyID { get; set; }

        public string? chargeName { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal chargeAmt { get; set; }

        public bool isVariable { get; set; }

        public bool isActive { get; set; }

        [ForeignKey(nameof(PropID))]
        public virtual XRS_Properties? Property { get; set; }
    }

}
