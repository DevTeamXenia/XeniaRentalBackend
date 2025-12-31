using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_Bedspace")]
    public class XRS_Bedspace
    {
        [Key]
        public int bedID { get; set; }

        public int companyID { get; set; }

        public int propID { get; set; }

        public int unitID { get; set; }

        public int planID { get; set; }

        public required string bedSpaceName { get; set; }

        public int bedSpaceCount { get; set; }

        public decimal rentAmt { get; set; }

        [NotMapped]
        public string? PropName { get; set; }

        [NotMapped]
        public string? UnitName { get; set; }

        public  bool isActive  {get; set; }

        [ForeignKey("propID")]
        [JsonIgnore]
        public virtual XRS_Properties? Properties { get; set; }

        [ForeignKey("unitID")]
        [JsonIgnore]
        public virtual XRS_Units? Units { get; set; }
    }
}
