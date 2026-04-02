using Microsoft.AspNetCore.SignalR;
using XeniaRentalBackend.Service.Maintenance;

namespace XeniaRentalBackend.Hubs
{
    public class MaintenanceHub : Hub
    {
        private readonly IMaintenanceUpdateService _maintenanceUpdateService;

        public MaintenanceHub(IMaintenanceUpdateService maintenanceUpdateService)
        {
            _maintenanceUpdateService = maintenanceUpdateService;
        }

        // ─────────────────────────────────────────
        // Company Group — Admin
        // ─────────────────────────────────────────
        public async Task JoinCompanyGroup(int companyId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"company-{companyId}");
        }

        public async Task LeaveCompanyGroup(int companyId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"company-{companyId}");
        }

        // ─────────────────────────────────────────
        // Employee Group — Employee App
        // ─────────────────────────────────────────
        public async Task JoinEmployeeGroup(int companyId, int employeeId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"company-{companyId}-employee-{employeeId}");
        }

        public async Task LeaveEmployeeGroup(int companyId, int employeeId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"company-{companyId}-employee-{employeeId}");
        }

        // ─────────────────────────────────────────
        // Get Maintenance List — Socket വഴി
        // Separate APIs-ന് പകരം ഇത് ഉപയോഗിക്കുന്നു
        // ─────────────────────────────────────────
        public async Task GetMaintenanceList(
            int companyId,
            string status,       // "pending" / "inprogress" / "completed" / "overdue"
            string? zone = null,
            string? search = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                await _maintenanceUpdateService.SendMaintenanceUpdate(
                    companyId,
                    status,
                    zone,
                    search,
                    pageNumber,
                    pageSize,
                    connectionId: Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== SignalR GetMaintenanceList Error ===");
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // ─────────────────────────────────────────
        // Broadcast — New Complaint Create ആകുമ്പോൾ
        // ─────────────────────────────────────────
        public async Task NotifyNewComplaint(int companyId, object complaintData)
        {
            await Clients
                .Group($"company-{companyId}")
                .SendAsync("NewComplaintReceived", complaintData);
        }
    }
}