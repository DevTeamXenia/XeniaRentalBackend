using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("XRS_ComplaintServiceNo")]
public class XRS_ComplaintServiceNo
{
    [Key]
    public int ServiceId { get; set; }
    public int MaintenanceId { get; set; }
    public int CompanyId { get; set; }

    [Required]
    [StringLength(50)]
    public string ServiceNo { get; set; } = string.Empty;

    public DateTime ServiceDate { get; set; } = DateTime.Now;
   
}