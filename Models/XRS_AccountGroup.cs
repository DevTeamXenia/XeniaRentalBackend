using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_AccountGroup")]
    public class XRS_AccountGroup
    {
        [Key]
        public int groupID { get; set; }

        public int companyID {  get; set; }

        public string groupCode { get; set; }

        public string groupName { get; set; }

        public bool isActive { get;set; }

        public DateTime createdOn { get; set; }

        public DateTime modifiedOn { get; set; }

        public string  modifiedBy { get; set; }

    }
}
