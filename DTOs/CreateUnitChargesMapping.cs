namespace XeniaRentalBackend.DTOs
{
    public class CreateUnitChargesMapping
    {
        public int chargeID { get; set; }

        public int unitID { get; set; }

        public int propID { get; set; }

        public int companyID { get; set; }

        public string frequency { get; set; }

        public decimal amount { get; set; }

        public bool isActive { get; set; }
    }
}
