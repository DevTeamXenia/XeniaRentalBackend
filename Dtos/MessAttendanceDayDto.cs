namespace XeniaRentalBackend.Dtos
{
    public class MessAttendanceDayDto
    {
        public int Day { get; set; }
        public bool IsPresent { get; set; }
    }

    public class MessTypeAttendanceDto
    {
        public int MessTypeId { get; set; }
        public string MessName { get; set; } = string.Empty;
        public string MessCode { get; set; } = string.Empty;
        public List<MessAttendanceDayDto> Days { get; set; } = new();
    }

    public class TenantMessAttendanceDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public int TenantAssignId { get; set; }
        public List<MessTypeAttendanceDto> MessTypes { get; set; } = new();
    }


}
