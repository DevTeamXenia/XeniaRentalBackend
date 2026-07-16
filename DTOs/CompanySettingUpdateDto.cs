using XeniaRentalBackend.Dtos;

namespace XeniaRentalBackend.DTOs
{
    public class CompanySettingUpdateDto
    {
        public int companyID { get; set; }

        public string companyName { get; set; } = string.Empty;

        public string address { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public string pin { get; set; }

        public string? logo { get; set; }

        public bool IsActive { get; set; }
        public List<UpdateCompanySettinsDto> CompanyDetails { get; set; } = new();
    }
    public class UpdateCompanySettinsDto
    {
        public string KeyCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

}
