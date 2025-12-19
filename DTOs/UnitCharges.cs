namespace XeniaRentalBackend.DTOs
{
    public class UnitCharges
    {
        public CreateUnit Unit { get; set; }
        public List<CreateUnitChargesMapping> Charges { get; set; }
    }
}
