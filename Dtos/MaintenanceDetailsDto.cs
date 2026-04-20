using XeniaRentalBackend.Dtos;

namespace XeniaTenoraBackend.Dtos
{
    public class MaintenanceDetailsDto
    {
        public MaintenanceResponseDto Current { get; set; }
        public List<MaintenanceResponseDto> History { get; set; }
    }
}
