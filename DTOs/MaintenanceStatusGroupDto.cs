using XeniaRentalBackend.Dtos;

namespace XeniaTenoraBackend.Dtos
{
    public class MaintenanceStatusGroupDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public List<MaintenanceResponseDto> Data { get; set; } = new();
    }
}
