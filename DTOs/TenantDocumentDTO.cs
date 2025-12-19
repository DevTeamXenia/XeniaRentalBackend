namespace XeniaRentalBackend.Dtos
{
    public class TenantDocumentDto
    {
        public int TenantID { get; set; }
        public int DocTypeId { get; set; }
        public int CompanyID { get; set; }
        public string DocumentsNo { get; set; }
        public string DocumentUrl { get; set; }
        public bool IsActive { get; set; }
        public string DocumentName { get; set; }
        public bool IsAlphaNumeric { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsExpiry { get; set; }
        public string DocPurpose { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
