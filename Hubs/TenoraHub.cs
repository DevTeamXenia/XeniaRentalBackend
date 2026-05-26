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
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"company-{companyId}");
                Console.WriteLine($"✅ Connected {Context.ConnectionId} to company-{companyId}");
            }
            catch (Exception ex)
            {
                throw new HubException("JoinCompanyGroup failed: " + ex.Message);
            }
        }

        public async Task LeaveCompanyGroup(int companyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"company-{companyId}");
        }

        public async Task JoinEmployeeGroup(int companyId, int? employeeId)
        {
            try
            {
                if (!employeeId.HasValue)
                    throw new HubException("EmployeeId is required");

                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"company-{companyId}-employee-{employeeId.Value}"
                );

                Console.WriteLine($"✅ Connected {Context.ConnectionId} to employee-{employeeId}");
            }
            catch (Exception ex)
            {
                throw new HubException("JoinEmployeeGroup failed: " + ex.Message);
            }
        }

        public async Task LeaveEmployeeGroup(int companyId, int employeeId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"company-{companyId}-employee-{employeeId}"
            );
        }

        public async Task SendTenoraUpdate(
            int companyId,
            int? tenantId = null,
            int? employeeId = null,
            int? pageNumber = null,
            int? pageSize = null,
            string? search = null,
            string? status = null
        )
        {
            try
            {
                Console.WriteLine($"📤 SendTenoraUpdate -> Company:{companyId}, Tenant:{tenantId}, Employee:{employeeId}");

                await _tenoraUpdateService.SendTenoraUpdate(
                    companyId,
                    tenantId,
                    employeeId,
                    pageNumber,
                    pageSize,
                    search,        
                    Context.ConnectionId
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 HUB ERROR: " + ex.ToString());
                throw new HubException(ex.Message);
            }
        }
    }
}