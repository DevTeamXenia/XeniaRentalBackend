namespace XeniaTenoraBackend.Dtos
{
    public class PropertyListDto
    {
        public int PropID { get; set; }

        public string propertyName { get; set; } = string.Empty;

        public string propertyType { get; set; } = string.Empty;

        public string? propertyPrefix { get; set; }

        public int? propertyAreaId { get; set; }

        public string? AreaName { get; set; }

        public bool IsActive { get; set; }

        public int CompanyId { get; set; }
    }
}
