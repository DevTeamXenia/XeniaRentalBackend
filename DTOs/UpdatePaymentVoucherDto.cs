namespace XeniaTenoraBackend.Dtos
{
    public class UpdatePaymentVoucherDto
    {
        public int voucherId { get; set; }

        public int companyId { get; set; }

        public int tenantId { get; set; }

        public decimal amount { get; set; }
        public int rentYear { get; set; }
        public int rentMonth { get; set; }
        public int unitId { get; set; }
        public int propId { get; set; }

        public DateTime VoucherDate { get; set; }

        public string VoucherStatus { get; set; } = string.Empty;

        public string? RefNo { get; set; }

        public string? Remarks { get; set; }

        public string? IssuingBank { get; set; }

        public string? ChequeNo { get; set; }

        public bool Cancelled { get; set; }

        public bool IsReconcil { get; set; }

        public bool? ChequeStatus { get; set; }

        public DateTime? ReconcilDate { get; set; }

        public string? ModificationBy { get; set; }
    }
}
