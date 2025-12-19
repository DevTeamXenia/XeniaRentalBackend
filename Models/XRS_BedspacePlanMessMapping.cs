using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_BedspacePlanMessMapping")]
    public class XRS_BedspacePlanMessMapping
    {
        [Key]
        public int bpmID { get; set; }
        public int companyID { get; set; }
        public int bedPlanID { get; set; }
        public int messID { get; set; }
        public bool active { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(bedPlanID))]
        public XRS_BedSpacePlan? BedSpacePlan { get; set; }
    }
}
