namespace XeniaRentalBackend.Dtos
{
    public class TenantChequeListDto
    {
        public int ChequeRegisterId { get; set; }
        public int PropID { get; set; }
        public int UnitID { get; set; }
        public int TenantID { get; set; }
        public string? TenantName { get; set; }
        public string? ChequeNo { get; set; }
        public string? IssueBank { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? ChequeUrl { get; set; }
        public string? Status { get; set; }
        public bool Active { get; set; }
    }
}
