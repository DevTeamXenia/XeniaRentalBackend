namespace XeniaRentalBackend.Dtos
{
    public class VoucherCreateRequest
    {
        public int UnitID { get; set; }
        public int CompanyID { get; set; }
        public int PropID { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public int DrID { get; set; }
        public int CrID { get; set; }
        public string? createdBy { get; set; }
        public string? modifiedBy { get; set; }
        public decimal Amount { get; set; }
        public string? RefNo { get; set; }
        public string? Remarks { get; set; }
        public string? VoucherStatus { get; set; }
        public bool IsActive { get; set; } = true;

        public List<VoucherDetailCreateRequest> VoucherDetails { get; set; } = new();
    }

    public class VoucherDetailCreateRequest
    {
        public int ChargeId { get; set; }
        public decimal Amount { get; set; }
    }
}
