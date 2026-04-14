using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_PropertyAreas")]
    public class XRS_PropertyAreas
    {
        [Key]
        public int Id { get; set; }

        public int PropId { get; set; }

        public int AreaId { get; set; }

        [ForeignKey("AreaId")]
        public XRS_Area Area { get; set; }
    }
}