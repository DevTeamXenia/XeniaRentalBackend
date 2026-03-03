namespace XeniaRentalBackend.Dtos
{
    public class RenewSubscriptionDto
    {
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
    }
}
