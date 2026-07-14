namespace XeniaRentalBackend.Dtos
{
    public class PlanWithModulesDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PlanDescription { get; set; } = string.Empty;
        public int PlanUsers { get; set; }
        public List<PlanDurationDto> Durations { get; set; } = new();
        public List<ModuleDto> Modules { get; set; } = new();
    }
    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string? ModuleDescription { get; set; } = string.Empty;
        public bool ModuleActive { get; set; }
    }


    public class PlanDurationDto
    {
        public int PlanDurationId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public decimal DPrice { get; set; }
        public decimal CPrice { get; set; }
    }


    public class AddonPlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal? PlanPrice { get; set; }
        public decimal? PlanDPrice { get; set; }
        public decimal? PlanCPrice { get; set; }
        public int PlanUsers { get; set; }
    }
}
