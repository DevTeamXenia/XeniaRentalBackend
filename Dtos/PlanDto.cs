namespace XeniaRentalBackend.Dtos
{
    public class PlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string? PlanDescription { get; set; } = string.Empty;
        public decimal? PlanPrice { get; set; }
        public int? PlanDurationDays { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }
        public List<ModuleDto> Modules { get; set; } = new();
    }
}
