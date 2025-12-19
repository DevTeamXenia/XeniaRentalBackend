using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Category")]
    public class XRS_Categories
    {
        [Key]
        public int CatID { get; set; }

        public int CompanyID { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<XRS_Units> Units { get; set; }

    }
}
