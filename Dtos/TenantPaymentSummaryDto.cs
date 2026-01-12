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
    }



    public class PaidPaymentDto
    {
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public decimal Amount { get; set; }
    }

}
