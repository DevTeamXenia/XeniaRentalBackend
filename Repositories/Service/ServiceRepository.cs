using Microsoft.EntityFrameworkCore;
using Stripe;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Common;


namespace XeniaRentalBackend.Repositories.Service
{
    public class ServiceRepository: IServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelperService _jwtHelperService;
        public ServiceRepository(ApplicationDbContext context, JwtHelperService jwtHelperService)
        {
            _context = context;
            _jwtHelperService = jwtHelperService;

        }

        public async Task<IEnumerable<XRS_Service>> GetServices(int companyId)
        {
            return await _context.Services
                .Where(p => p.serviceCompanyID == companyId) 
                .ToListAsync();
        }

        public async Task<PagedResultDto<XRS_Service>> GetServiceByCompanyId(int companyId,string? search = null, int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.Services.AsQueryable();

            query = query.Where(u => u.serviceCompanyID == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(u =>
                    u.serviceName.ToLower().Contains(lowerSearch));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.serviceName) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new XRS_Service
                {
                    serviceID = u.serviceID,
                    serviceName = u.serviceName,
                    servicePhoneNumber = u.servicePhoneNumber,
                    serviceWhatappNumber = u.serviceWhatappNumber,
                    serviceCompanyID = u.serviceCompanyID,
                    servicePropertyID = u.servicePropertyID,
                    ServiceStatus = u.ServiceStatus
                })
                .ToListAsync();

            return new PagedResultDto<XRS_Service>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<IEnumerable<XRS_Service>> GetServicesbyId(int serviceId)
        {

            return await _context.Services
                .Where(u => u.serviceID == serviceId)
                 .ToListAsync();

        }

        public async Task<XRS_Service> CreateServices(XRS_Service dtoServices)
        {

            var service = new XRS_Service
            {
                serviceName = dtoServices.serviceName,
                servicePhoneNumber = dtoServices.servicePhoneNumber,
                serviceWhatappNumber = dtoServices.serviceWhatappNumber,
                serviceCompanyID = dtoServices.serviceCompanyID,
                servicePropertyID = dtoServices.servicePropertyID,
                ServiceStatus = dtoServices.ServiceStatus,

            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return service;

        }

        public async Task<bool> UpdateServices(int id, XRS_Service service)
        {
            var updateServices = await _context.Services.FirstOrDefaultAsync(u => u.serviceID == id);
            if (updateServices == null) return false;

            updateServices.serviceName = service.serviceName;
            updateServices.serviceWhatappNumber = service.serviceWhatappNumber;
            updateServices.servicePhoneNumber = service.servicePhoneNumber;
            updateServices.ServiceStatus = service.ServiceStatus;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteService(int id)
        {
            var bedspacesettings = await _context.Properties.FirstOrDefaultAsync(u => u.PropID == id);
            if (bedspacesettings == null) return false;
            bedspacesettings.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<XRS_Service>> GetServiceByPropertyIdAsync(int propertyId, string serviceType)
        {
            return await _context.Services
                .Where(c => c.servicePropertyID == propertyId && c.serviceType == serviceType && c.ServiceStatus)
                .ToListAsync();
        }


    }
}
