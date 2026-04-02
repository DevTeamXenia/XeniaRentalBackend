namespace XeniaRentalBackend.Service.Maintenance
{
    public interface IMaintenanceUpdateService
    {
        // Complaint List Update — Admin-ലേക്ക്
        Task SendMaintenanceUpdate(
            int companyId,
            string status,
            string? zone = null,
            string? search = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? connectionId = null);
    }
}