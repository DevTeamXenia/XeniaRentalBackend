namespace XeniaTenoraBackend.DTOs
{
    public class CompanySettingsDto
    {
        public int CompanySettingsId { get; set; }
        public int CompanyId { get; set; }
        public string KeyCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
