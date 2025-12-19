namespace XeniaRentalBackend.Dtos
{
    public class TenantChequeRegisterDto
    {
        public int propID { get; set; }
        public int unitID { get; set; }
        public int tenantID { get; set; }
        public string? chequeNo { get; set; }
        public string? chequeUrl { get; set; }
        public DateTime? chequeDate { get; set; }
        public string? issueBank { get; set; }
        public decimal amount { get; set; }
        public string? status { get; set; }       
        public bool active { get; set; }
    }
}
