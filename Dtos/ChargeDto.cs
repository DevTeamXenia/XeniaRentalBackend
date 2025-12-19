namespace XeniaRentalBackend.Dtos
{
    public class ChargeDto
    {
        public int ChargeId { get; set; }
        public string ChargeName { get; set; }
        public decimal ChargeAmount { get; set; }
        public bool IsVariable { get; set; }
    }
}
