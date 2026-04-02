namespace XeniaRentalBackend.Dtos
{
    public class MaintenanceCategoryDto
    {
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public string CategoryName { get; set; }
        public int SLADays { get; set; }
        public int SLAHours { get; set; }
        public bool IsActive { get; set; }
    }
}
