namespace XeniaRentalBackend.DTOs
{
    public class AddAddonDto
    {
        public int CompanyId { get; set; }
        public List<int> AddonPlanIds { get; set; } = new();
    }
}
