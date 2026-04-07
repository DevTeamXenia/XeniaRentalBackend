using Microsoft.AspNetCore.SignalR;
using XeniaTenoraBackend.Service.Socket;


namespace XeniaTenoraBackend.Hubs
{
    public class TenoraHub : Hub
    {
        private readonly ITenoraUpdateService _tenoraUpdateService;

        public TenoraHub(ITenoraUpdateService tenoraUpdateService)
        {
            _tenoraUpdateService = tenoraUpdateService;
        }


        public async Task JoinCompanyGroup(int companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"company-{companyId}");
        }

        public async Task LeaveCompanyGroup(int companyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"company-{companyId}");
        }


        public async Task JoinEmployeeGroup(int companyId, int employeeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"company-{companyId}-employee-{employeeId}");
        }

        public async Task LeaveEmployeeGroup(int companyId, int employeeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"company-{companyId}-employee-{employeeId}");
        }


        public async Task SendTenoraUpdate(int companyId, int? tenanatId = null, int? id = null, int? pageNumber = null, int? pageSize = null, string? search = null, string? connectionId = null)
        {
            try
            {
                await _tenoraUpdateService.SendTenoraUpdate( companyId, tenanatId,  id, pageNumber, pageSize, search, connectionId: Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== SignalR SendCatalogueUpdate Error ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}