using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_UnitChargesMapping")]
    public class XRS_UnitChargesMapping
    {
        [Key]
        public int unitMapID { get; set; }

        public int chargeID { get; set; }

        public int unitID { get; set; }

        public int propID { get; set; }

        public int companyID { get; set; }

        public string frequency { get; set; }

        public decimal amount { get; set; }

        public bool isActive { get; set; }

        [ForeignKey(nameof(unitID))]
        public virtual XRS_Units? Unit { get; set; }

        [ForeignKey(nameof(chargeID))]
        public virtual XRS_Charges? Charges { get; set; }
    }

}

