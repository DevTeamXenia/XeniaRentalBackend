namespace XeniaRentalBackend.Dtos.Reports
{
    public class TenantOccupancyReportDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public List<UnitReportDto> Units { get; set; } = new();
    }

    public class UnitReportDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }

        public List<TenantRowDto>? Tenants { get; set; }
       
        public List<BedSpaceReportDto>? BedSpaces { get; set; }
    }

    public class BedSpaceReportDto
    {
        public int? BedSpaceId { get; set; }
        public string? BedSpaceName { get; set; }
        public List<TenantRowDto> Tenants { get; set; } = new();
    }

    public class TenantRowDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string Phone { get; set; }
        public decimal Deposit { get; set; }
        public decimal Rent { get; set; }
        public decimal Balance { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; }
    }
}
