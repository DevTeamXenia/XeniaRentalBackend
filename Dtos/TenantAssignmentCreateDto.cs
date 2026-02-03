namespace XeniaRentalBackend.Dtos
{
    public class TenantAssignmentCreateDto
    {
        public int propID { get; set; }
        public int unitID { get; set; }
        public int tenantID { get; set; }
        public int companyID { get; set; }
        public decimal securityAmt { get; set; }
        public bool isTaxable { get; set; }
        public int taxPercentage { get; set; }
        public int bedSpaceID { get; set; }
        public decimal rentAmt { get; set; }
        public int rentConcession { get; set; }
        public int messConcession { get; set; }
        public string? collectionType { get; set; }
        public DateTime agreementStartDate { get; set; }
        public DateTime agreementEndDate { get; set; }
        public DateTime closureDate { get; set; }
        public int rentCollection { get; set; }
        public int escalationPer { get; set; }
        public DateTime? nextescalationDate { get; set; }
        public DateTime rentDueDate { get; set; }
        public decimal refundAmount { get; set; }
        public decimal Charges { get; set; }
        public decimal amount { get; set; }
        public string? notes { get; set; }
        public string? paymentMode { get; set; }
        public bool isClosure { get; set; }
        public bool isActive { get; set; }

        public List<TenantCreateDocumentDto>? Documents { get; set; }
        public List<TenantChequeRegisterDto>? Cheques { get; set; }
    }
}
