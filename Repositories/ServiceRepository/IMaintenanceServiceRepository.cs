using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaTenoraBackend.DTOs;

namespace XeniaRentalBackend.Repositories.MaintenanceService
{
    public interface IMaintenanceServiceRepository
    {
        // Service No List — Complaint Details Page
        Task<List<object>> GetServicesByMaintenanceId(int maintenanceId);

        // Service History — Dropdown Click
        Task<object?> GetServiceHistory(int serviceId);

      
    }
}