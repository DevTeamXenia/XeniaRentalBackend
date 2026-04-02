using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.Dtos
{
    namespace XeniaRentalBackend.Dtos
    {
        public class ComplaintAssignmentDto
        {
            [Required]
            public int CompanyId { get; set; }

            [Required]
            public int MaintenanceId { get; set; }

            [Required]
            public int CategoryId { get; set; }

            public int? UpdatedCategoryId { get; set; }

            [Required]
            public int AssignedEmployeeId { get; set; }

            public string? Instructions { get; set; }
        }
    }
}