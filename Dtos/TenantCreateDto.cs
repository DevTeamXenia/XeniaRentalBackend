namespace XeniaRentalBackend.Dtos
{
    public class TenantCreateDto
    {
        public int companyID { get; set; }
        public string tenantName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string emergencyContactNo { get; set; }
        public decimal concessionper { get; set; }
        public string note { get; set; }
        public string address { get; set; }
        public bool isActive { get; set; }

        public List<TenantCreateDocumentDto>? Documents { get; set; }
    }


    public class TenantCreateDocumentDto
    {
        public int docTypeId { get; set; }
        public string documentsNo { get; set; }
        public string documenturl { get; set; }
        public bool isActive { get; set; }
    }

}
