using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace XeniaRentalBackend.Models
{
    [Table("XRS_TenantChequeRegister")]
    public class XRS_TenantChequeRegister
    {
        [Key]
        public int chequeRegisterId { get; set; }

        [Required]
        public int propID { get; set; }

        [Required]
        public int unitID { get; set; }

        [Required]
        public int tenantID { get; set; }

        [MaxLength(100)]
        public string? chequeNo { get; set; }

        [MaxLength(500)]
        public string? chequeUrl { get; set; }

        public DateTime? chequeDate { get; set; }

        [MaxLength(150)]
        public string? issueBank { get; set; }

        public decimal amount { get; set; }

        [MaxLength(500)]
        public string? status { get; set; }

        public bool active { get; set; } = true;

        [ForeignKey("tenantID")]
        [JsonIgnore]
        public virtual XRS_Tenant? Tenant { get; set; }
    }
}

