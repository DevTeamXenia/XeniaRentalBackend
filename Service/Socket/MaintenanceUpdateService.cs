using Microsoft.AspNetCore.SignalR;
using XeniaRentalBackend.Repositories.ManageMaintenance;
using XeniaRentalBackend.Service.Hubs;

namespace XeniaRentalBackend.Service.Socket
{
    public interface IMaintenanceUpdateService
    {
        Task SendMaintenanceUpdate(int companyId, string? search = null, DateTime? date = null, int pageNumber = 1, int pageSize = 10, string? connectionId = null);
    }

    public class MaintenanceUpdateService : IMaintenanceUpdateService
    {
        private readonly IHubContext<MaintenanceHub> _hubContext;
        private readonly IManageMaintenanceRepository _maintenanceRepo;

        public MaintenanceUpdateService(
            IHubContext<MaintenanceHub> hubContext,
            IManageMaintenanceRepository maintenanceRepo)
        {
            _hubContext = hubContext;
            _maintenanceRepo = maintenanceRepo;
        }

        public async Task SendMaintenanceUpdate(int companyId, string? search = null, DateTime? date = null, int pageNumber = 1, int pageSize = 10, string? connectionId = null)
        {
            var pending = await _maintenanceRepo.GetPendingList(companyId, search, date, pageNumber, pageSize);
            var inProgress = await _maintenanceRepo.GetInProgressList(companyId, search, date, pageNumber, pageSize);
            var completed = await _maintenanceRepo.GetCompletedList(companyId, search, date, pageNumber, pageSize);

            var dashboardData = new
            {
                Pending = pending,
                InProgress = inProgress,
                Completed = completed
            };

            var target = !string.IsNullOrEmpty(connectionId)
                ? _hubContext.Clients.Client(connectionId)
                : _hubContext.Clients.Group($"company-{companyId}");

            await target.SendAsync("ReceiveMaintenanceUpdate", dashboardData);
        }
    }
}
