using Microsoft.AspNetCore.SignalR;
using XeniaRentalBackend.Hubs;
using XeniaRentalBackend.Repositories.AdminComplaint;

namespace XeniaRentalBackend.Service.Maintenance
{
    public class MaintenanceUpdateService : IMaintenanceUpdateService
    {
        private readonly IHubContext<MaintenanceHub> _hubContext;
        private readonly IAdminComplaintRepository _adminComplaintRepository;

        public MaintenanceUpdateService(
            IHubContext<MaintenanceHub> hubContext,
            IAdminComplaintRepository adminComplaintRepository)
        {
            _hubContext = hubContext;
            _adminComplaintRepository = adminComplaintRepository;
        }

        public async Task SendMaintenanceUpdate(
            int companyId,
            string status,
            string? zone = null,
            string? search = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? connectionId = null)
        {
            // Status-ൽ നിന്ന് Data Fetch ചെയ്യുക
            object? data = status.ToLower() switch
            {
                "pending" => await _adminComplaintRepository
                    .GetNewComplaints(companyId, search, zone, pageNumber, pageSize),
                "inprogress" => await _adminComplaintRepository
                    .GetInProgressComplaints(companyId, search, zone, pageNumber, pageSize),
                "completed" => await _adminComplaintRepository
                    .GetClosedComplaints(companyId, search, zone, pageNumber, pageSize),
                "overdue" => await _adminComplaintRepository
                    .GetOverdueComplaints(companyId, search, zone, pageNumber, pageSize),
                _ => null
            };

            if (data == null) return;

            var payload = new
            {
                Status = status,
                Data = data
            };

            // Specific Connection-ലേക്ക് Send ചെയ്യുക
            if (!string.IsNullOrEmpty(connectionId))
            {
                await _hubContext.Clients
                    .Client(connectionId)
                    .SendAsync("ReceiveMaintenanceUpdate", payload);
            }
            else
            {
                // Company Group-ലേക്ക് Broadcast ചെയ്യുക
                await _hubContext.Clients
                    .Group($"company-{companyId}")
                    .SendAsync("ReceiveMaintenanceUpdate", payload);
            }
        }
    }
}