namespace XeniaRentalBackend.Dtos
{
    public class MarkAttendanceDto
    {
        public int TenantId { get; set; }
        public int TenantAssignId { get; set; }
        public int BedSpacePlanId { get; set; }
        public int MessTypeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public string? CreatedBy { get; set; }
    }
}
