using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalApi.Models
{
    [Table("XRS_AdvText")]
    public class XRS_AdvText
    {
        [Key]
        public int textID { get; set; }

        public int companyID { get; set; }

        public int propertyID { get; set; }

        public required string textContent { get; set; }

        public bool textStatus { get; set; }
    }
}