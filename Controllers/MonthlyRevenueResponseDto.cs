using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.Controllers
{
    public class MonthlyRevenueResponseDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    }
}
