using Microsoft.AspNetCore.SignalR;
using XeniaRentalBackend.Service.Socket;

namespace XeniaRentalBackend.Service.Hubs
{
    public class MaintenanceHub : Hub
    {
        private readonly IMaintenanceUpdateService _maintenanceUpdateService;

        public MaintenanceHub(IMaintenanceUpdateService maintenanceUpdateService)
        {
            _maintenanceUpdateService = maintenanceUpdateService;
        }

        public async Task JoinCompanyGroup(int companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"company-{companyId}");
        }

        public async Task LeaveCompanyGroup(int companyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"company-{companyId}");
        }

        public async Task SendMaintenanceUpdate(int companyId, string? search = null, DateTime? date = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                await _maintenanceUpdateService.SendMaintenanceUpdate(companyId, search, date, pageNumber, pageSize, connectionId: Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== SignalR SendMaintenanceUpdate Error ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
