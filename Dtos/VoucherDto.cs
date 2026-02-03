namespace XeniaRentalBackend.Dtos
{
    public class VoucherDto
    {
        public int UnitID { get; set; }
        public int CompanyID { get; set; }
        public int PropID { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; } = string.Empty;
        public string DrID { get; set; }
        public int CrID { get; set; }
        public decimal Amount { get; set; }
        public string? RefNo { get; set; }
        public string? Remarks { get; set; }
        public string? IssuingBank { get; set; }
        public string? ChequeNo { get; set; }
        public bool Cancelled { get; set; } = false;
        public decimal? CrAmount { get; set; }
        public bool IsReconcil { get; set; } = false;
        public bool? ChequeStatus { get; set; }
        public DateTime? ReconcilDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModificationBy { get; set; }
        public string? PaidByUser { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
