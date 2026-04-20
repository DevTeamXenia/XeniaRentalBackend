using System;

namespace XeniaRentalBackend.Dtos
{
    public class MaintenanceReportDto
    {
        public int MaintenanceId { get; set; }
        public string ComplaintNo { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; } 
        public string PropertyUnit { get; set; } = string.Empty;
        public string RegisteredBy { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty; 
        public string Complaint { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EngineerName { get; set; } = string.Empty; 
        public string Zone { get; set; } = string.Empty; 
        public DateTime UpdatedAt { get; set; } 
    }
}
