using System.ComponentModel.DataAnnotations;

namespace XeniaRentalBackend.Models
{
    public class XRS_Agent
    {
        [Key]
        public int AgentId { get; set; }
        public int CompanyId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string AgentCode { get; set; } = string.Empty;
        public bool AgentActive { get; set; }
    }
}
