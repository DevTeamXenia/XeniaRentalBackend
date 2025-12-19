namespace XeniaRentalBackend.Dtos
{
    public class TenantAssignmentGetDto
    {
        public int tenantAssignId { get; set; }
        public int propID { get; set; }
        public string? PropName { get; set; }
        public int unitID { get; set; }
        public string? UnitName { get; set; }
        public int tenantID { get; set; }
        public string? TenantName { get; set; }
        public string? TenantContactNo { get; set; }
        public decimal rentAmt { get; set; }
        public decimal securityAmt { get; set; }
        public bool isTaxable { get; set; }
        public int taxPercentage { get; set; }
        public string collectionType { get; set; }
        public int rentCollection { get; set; }
        public int escalationPer { get; set; }
        public DateTime? nextescalationDate { get; set; }
        public DateTime rentDueDate { get; set; }
        public decimal refundAmount { get; set; }
        public decimal dueAmount { get; set; }
        public decimal charges { get; set; }
        public decimal amount { get; set; }
        public decimal rentConcession { get; set; }
        public decimal messConcession { get; set; }
        public DateTime agreementStartDate { get; set; }
        public DateTime agreementEndDate { get; set; }
        public bool isActive { get; set; }
        public bool isClosure { get; set; }
        public string? paymentMode { get; set; }
        public DateTime? closureDate { get; set; }
        public string closureReason { get; set; }
        public string? notes { get; set; }
        public int? BedSpaceID { get; set; }
        public string? BedSpaceName { get; set; }
        public List<TenantDocumentDto> Documents { get; set; }
        public List<TenantChequeRegisterDto> Cheques { get; set; }
    }
}
