namespace XeniaRentalBackend.Dtos
{
    public class PropertyWithUnitsDto
    {
        public int PropID { get; set; }
        public string PropertyName { get; set; }
        public List<UnitPropertyDto> Units { get; set; } = new();
    }

    public class UnitPropertyDto
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
    }
}
