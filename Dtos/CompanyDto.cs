namespace XeniaRentalBackend.Dtos
{
    public class CompanyDto
    {
        public int companyID { get; set; }

        public string companyName { get; set; } = string.Empty;

        public string address { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public string pin { get; set; }

        public string? logo { get; set; }

        public bool IsActive { get; set; }
    }
}
