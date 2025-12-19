namespace XeniaRentalBackend.Dtos
{
    public class TenantProfileDto
    {
        public int TenantID { get; set; }
        public string TenantName { get; set; }
        public int CompanyID { get; set; }
        public int PropID { get; set; }
        public int UnitID { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EmergencyContactNo { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
    }
}
