using System.Collections.Generic;

namespace XeniaRentalBackend.Dtos
{
    public class MaintenanceDashboardDto
    {
        public int NewComplaints { get; set; }
        public int InProgress { get; set; }
        public int Closed { get; set; }
        public int Overdue { get; set; }
        public List<PropertyComplaintStatsDto> PropertyStats { get; set; } = new();
    }

    public class PropertyComplaintStatsDto
    {
        public string PropertyName { get; set; } = string.Empty;
        public int Complaints { get; set; }
        public int Solved { get; set; }
    }
}
