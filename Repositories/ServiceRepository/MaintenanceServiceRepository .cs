using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaTenoraBackend.DTOs;

namespace XeniaRentalBackend.Repositories.MaintenanceService
{
    public class MaintenanceServiceRepository : IMaintenanceServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────
        // Service No List — Complaint Details Page-ൽ
        public async Task<List<object>> GetServicesByMaintenanceId(int maintenanceId)
        {
            var services = await _context.ComplaintServiceNo
                .Where(s => s.MaintenanceId == maintenanceId)
                .OrderByDescending(s => s.ServiceDate)
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceNo,
                    Date = s.ServiceDate.ToString("dd/MM/yyyy")
                })
                .ToListAsync();

            return services.Cast<object>().ToList();
        }
        // ─────────────────────────────────────────
        // Service History — Dropdown Click ചെയ്താൽ
        // ─────────────────────────────────────────
        public async Task<object?> GetServiceHistory(int serviceId)
        {
            var service = await _context.ComplaintServiceNo
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null) return null;

            var history = await _context.MaintenanceServiceHistories
                .Where(h => h.ServiceId == serviceId)
                .OrderBy(h => h.ReportDate)
                .Select(h => new
                {
                    Date = h.ReportDate.ToString("dd/MM/yyyy"),
                    Time = h.ReportDate.ToString("hh:mm tt"),
                    h.Complaint,
                    h.Report,
                    h.CreatedBy
                })
                .ToListAsync();

            return new
            {
                service.ServiceId,
                service.ServiceNo,
                Date = service.ServiceDate.ToString("dd/MM/yyyy"),
                History = history
            };
        }

     
   

    }
}