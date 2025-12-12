namespace XeniaRentalApi.Dtos
{
    public class TenantPaymentBannerDto
    {
        public List<PaymentInfoDto> LatestPayments { get; set; }
        public List<PaymentInfoDto> UpcomingPayments { get; set; }
        public List<BannerDto> ActiveBanners { get; set; }
        public List<AdvTextDto> ActiveTexts { get; set; }
    }

    public class PaymentInfoDto
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class BannerDto
    {
        public int bannerID { get; set; }
        public string bannerName { get; set; }
        public string bannerImage { get; set; }
    }

    public class AdvTextDto
    {
        public int textID { get; set; }
        public string textContent { get; set; }
    }
}
