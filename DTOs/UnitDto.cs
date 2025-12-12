namespace XeniaRentalApi.Dtos
{
    public class UnitDto
    {
        public int UnitId { get; set; }
        public int CompanyId { get; set; }
        public int PropID { get; set; }
        public required string UnitName { get; set; }
        public string? UnitType { get; set; }
        public bool IsActive { get; set; }
        public string? FloorNo { get; set; }
        public string? Area { get; set; }
        public string? Remarks { get; set; }
        public decimal DefaultRent { get; set; }
        public decimal escalationper { get; set; }
        public int? CatID { get; set; }
        public string? PropName { get; set; }
        public string? CategoryName { get; set; }
        public List<UnitChargeDto>? UnitCharges { get; set; }
    }

    public class UnitChargeDto
    {
        public int unitMapID { get; set; }
        public int chargeID { get; set; }
        public decimal amount { get; set; }
        public string frequency { get; set; }
        public bool isActive { get; set; }
        public string? ChargeName { get; set; }
        public string? ChargeType { get; set; }
    }

}
