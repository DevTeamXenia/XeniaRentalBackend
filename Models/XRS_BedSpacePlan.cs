using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_BedSpacePlan")]
    public class XRS_BedSpacePlan
    {
        [Key]
        public int bedPlanID { get; set; }
        public int companyID { get; set; }
        public string planName { get; set; } = string.Empty;
        public bool enableTax { get; set; }
        public int calculatedays { get; set; }
        public bool enableAdjustRent { get; set; }
        public decimal calculateAdjustRent { get; set; }
        public decimal calculatePartialRent { get; set; }
        public bool enableMess { get; set; }
        public decimal messCharge { get; set; }
        public bool includeMess { get; set; }
        public int messChargeDays { get; set; }
        public int consumedDays { get; set; }
        public bool isActive { get; set; }
        public ICollection<XRS_BedspacePlanMessMapping> BedspacePlanMessMappings { get; set; }
            = new List<XRS_BedspacePlanMessMapping>();
    }

}
