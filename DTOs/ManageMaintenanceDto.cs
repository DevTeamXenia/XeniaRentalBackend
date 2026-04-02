using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.Dtos
{
    // Create New Request
    public class ManageMaintenanceDto
    {
        [Required]
        public int CompanyId { get; set; }

        //[Required]
        //public int ComplaintNo { get; set; }
    

        [Required]
        public int PropertyId { get; set; }
    
        [Required]
        public string Unit { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Complaint { get; set; } = string.Empty;

        [Required]
        public string PreferredVisitTime { get; set; } = string.Empty;
        [Required]
        public int  ?TenantId { get; set; }  

        // Multiple Images
        public List<IFormFile>? Photos { get; set; }
    }

    // Update Status
    public class UpdateMaintenanceStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
        // "Pending" / "InProgress" / "Completed"

        public int? AssignedEmployeeId { get; set; }
    }
}
