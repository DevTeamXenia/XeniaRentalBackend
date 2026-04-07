
using Microsoft.AspNetCore.SignalR;
using Stripe;
using XeniaRentalBackend.Repositories.ManageMaintenance;
using XeniaTenoraBackend.Hubs;

namespace XeniaTenoraBackend.Service.Socket
{
    public class TenoraUpdateService : ITenoraUpdateService
    {
        private readonly IMaintenanceRepository _manageMaintenanceRepository;
        private readonly IHubContext<TenoraHub> _hubContext;

        public TenoraUpdateService( IMaintenanceRepository manageMaintenanceRepository,    IHubContext<TenoraHub> hubContext)
        {
            _manageMaintenanceRepository = manageMaintenanceRepository;
            _hubContext = hubContext;
        }

        //public async Task SendTenoraUpdate(int companyId, int? tenanatId = null, int? id = null, int? pageNumber = null, int? pageSize = null, string? search = null, string? connectionId = null)
        //{
        //    object? data = null;

        //    var maintenances = await _manageMaintenanceRepository.GetMaintenance(companyId, tenanatId);
        //    data = new
        //    {
        //        Maintance = maintenances,
        //    };


        //    if (id.HasValue)
        //    {
        //        await _hubContext.Clients.Group($"company-{companyId}-employee-{id.Value}")
        //            .SendAsync("ReceiveMaintenanceUpdate", data);
        //    }

        //    else
        //    {
        //        await _hubContext.Clients.Group($"company-{companyId}")
        //            .SendAsync("ReceiveMaintenanceUpdate", data);
        //    }
        //}  
        public async Task SendTenoraUpdate(int companyId, int? tenanatId = null, int? id = null, int? pageNumber = null, int? pageSize = null, string? search = null, string? connectionId = null)
        {
            try
            {
                var maintenances = await _manageMaintenanceRepository.GetMaintenance(companyId, tenanatId);

                var data = new
                {
                    Maintance = maintenances,
                };

                if (id.HasValue)
                {
                    await _hubContext.Clients.Group($"company-{companyId}-employee-{id.Value}")
                        .SendAsync("ReceiveMaintenanceUpdate", data);
                }
                else
                {
                    await _hubContext.Clients.Group($"company-{companyId}")
                        .SendAsync("ReceiveMaintenanceUpdate", data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 SERVICE ERROR: " + ex.ToString());
                throw;
            }
        }
    }

}
