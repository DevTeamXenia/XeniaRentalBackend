namespace XeniaRentalBackend.Dtos
{
    public class PlanWithModulesDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PlanDescription { get; set; } = string.Empty;
        public List<PlanDurationDto> Durations { get; set; } = new();
        public List<ModuleDto> Modules { get; set; } = new();
    }

    public class PlanDurationDto
    {
        public int PlanDurationId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
    }
}
