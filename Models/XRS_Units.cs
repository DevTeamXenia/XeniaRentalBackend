using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Units")]
    public class XRS_Units
    {
        [Key]
        public int UnitId { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(Property))]
        public int PropID { get; set; }

        [Required]
        [StringLength(50)]
        public string UnitName { get; set; } = string.Empty;

        public string? UnitType { get; set; }

        public bool IsActive { get; set; }

        public string? FloorNo { get; set; }

        public string? Area { get; set; }

        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DefaultRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal escalationper { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CatID { get; set; }


        public virtual XRS_Properties? Property { get; set; }
        public virtual XRS_Categories? Category { get; set; }

        [NotMapped]
        public string? PropName { get; set; }

        [NotMapped]
        public string? CategoryName { get; set; }

        [NotMapped]
        public List<XRS_UnitChargesMapping>? UnitCharges { get; set; }
    }
}
