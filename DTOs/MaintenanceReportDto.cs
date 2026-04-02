using System;

namespace XeniaRentalBackend.Dtos
{
    public class MaintenanceReportDto
    {
        public int MaintenanceId { get; set; }
        public string ComplaintNo { get; set; } = string.Empty; // Complaint ID
        public DateTime CreatedAt { get; set; } // Date
        public string PropertyUnit { get; set; } = string.Empty; // e.g., "Tower A - Flat 102"
        public string RegisteredBy { get; set; } = string.Empty; // Tenant Name
        public string CategoryName { get; set; } = string.Empty; // Category
        public string Complaint { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EngineerName { get; set; } = string.Empty; // Assigned Engineer
        public string Zone { get; set; } = string.Empty; // Assigned Employee's Zone
        public DateTime UpdatedAt { get; set; } // Updated Date
    }
}
