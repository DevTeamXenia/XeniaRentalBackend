namespace XeniaRentalBackend.Dtos
{
    public class TenantPaymentSummaryDto
    {
        public List<UpcomingPaymentDto> PreviousUnpaidPayments { get; set; }
        public UpcomingPaymentDto NextUpcomingPayment { get; set; }
        public List<PaidPaymentDto> PreviousPaidPayments { get; set; }
    }


    public class UpcomingPaymentDto
    {
        public string RentDueDate { get; set; }  
        public decimal RentAmount { get; set; }
        public string Remarks { get; set; }
        public List<ChargeDetailDto> Charges { get; set; } = new();
    }



    public class PaidPaymentDto
    {
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public decimal Amount { get; set; }
    }

    public class ChargeDetailDto
    {
        public int ChargeId { get; set; }

        public string ChargeName { get; set; }

        public decimal ChargeAmount { get; set; }

        public bool IsVariable { get; set; }

        public string Status { get; set; }
    }

}
