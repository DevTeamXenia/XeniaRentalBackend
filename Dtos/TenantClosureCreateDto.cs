namespace XeniaRentalBackend.Dtos
{
    public class TenantClosureCreateDto
    {
        public bool isClosure { get; set; }
        public DateTime closureDate { get; set; }
        public string? closureReason { get; set; }
        public decimal refundAmount { get; set; }
        public string? note { get; set; }
    }
}
