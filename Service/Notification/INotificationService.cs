namespace XeniaRentalBackend.Service.Notification
{
    public interface INotificationService
    {
        Task<string> SendNotification(int companyId, int? branchId, string notificationType, string mobileNo, string email, string emailSubject, Dictionary<string, string> parameters);
    }
}
