namespace XeniaRentalBackend.Dtos
{
    public class RentDashboardDto
    {
        public int TotalPaidCount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public int TotalNotPaidCount { get; set; }
        public decimal TotalNotPaidAmount { get; set; }
        public int TotalOccupiedProperties { get; set; }
        public int TotalOccupiedUnits { get; set; }
        public int VacantProperties { get; set; }
        public int VacantUnits { get; set; }
        public int TopPerformingPropertyCount { get; set; }
        public decimal AverageRentPerProperty { get; set; }
        public int HighRiskTenantCount { get; set; }
        public int HighRiskPropertyCount { get; set; }
        public decimal OccupancyRate { get; set; }   
        public decimal CollectionRate { get; set; }
    }
}
