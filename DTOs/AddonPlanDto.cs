namespace XeniaTenoraBackend.DTOs
{
    public class AddonPlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal? PlanPrice { get; set; }
        public int PlanUsers { get; set; }
    }
}
