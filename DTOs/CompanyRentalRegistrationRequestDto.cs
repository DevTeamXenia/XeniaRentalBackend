using System.ComponentModel.DataAnnotations;

namespace XeniaTenoraBackend.DTOs
{
    public class CompanyRentalRegistrationRequestDto
    {
        public required string companyName { get; set; }

        public string address { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public string pin { get; set; }

        public string? logo { get; set; }

        public bool IsActive { get; set; }
        public string? Country { get; set; }

        public string? userName { get; set; }
        public string? password { get; set; }

        public List<CompanyRentalSettingsDto> Settings { get; set; } = new();

    }

    public class CompanyRentalSettingsDto
    {
        public string KeyCode { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
