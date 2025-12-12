namespace XeniaRentalApi.Dtos
{
    public class BedSpaceDto
    {
        public int companyID { get; set; }

        public int propID { get; set; }

        public int unitID { get; set; }

        public int planID { get; set; }

        public string bedSpaceName { get; set; }

        public decimal rentAmt { get; set; }

        public bool isActive { get; set; }
    }
}
