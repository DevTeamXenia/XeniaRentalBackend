namespace XeniaRentalBackend.Dtos
{
    public class PlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PlanDescription { get; set; } = string.Empty;
        public decimal PlanPrice { get; set; }
        public int PlanDurationDays { get; set; }
        public bool PlanIsAddOn { get; set; }
        public bool PlanActive { get; set; }
        public List<ModuleDto> Modules { get; set; } = new();
    }

    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleDescription { get; set; } = string.Empty;
        public bool ModuleActive { get; set; }
    }
}
