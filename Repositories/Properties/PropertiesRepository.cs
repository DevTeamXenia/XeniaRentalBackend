using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Common;


namespace XeniaRentalBackend.Repositories.Properties
{
    public class PropertiesRepository : IPropertiesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelperService _jwtHelperService;
        public PropertiesRepository(ApplicationDbContext context, JwtHelperService jwtHelperService)
        {
            _context = context;
            _jwtHelperService = jwtHelperService;

        }

        public async Task<IEnumerable<XRS_Properties>> GetProperties(int companyId)
        {
            return await _context.Properties
                .Where(p => p.CompanyId == companyId) 
                .ToListAsync();
        }

        public async Task<PagedResultDto<XRS_Properties>> GetPropertiesByCompanyId(int companyId,string? search = null, int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.Properties.AsQueryable();

            query = query.Where(u => u.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(u =>
                    u.propertyName.ToLower().Contains(lowerSearch) ||
                    u.propertyType.ToLower().Contains(lowerSearch));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.propertyName) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new XRS_Properties
                {
                    PropID = u.PropID,
                    propertyName = u.propertyName,
                    propertyType = u.propertyType,
                    IsActive = u.IsActive,
                    CompanyId = u.CompanyId
                })
                .ToListAsync();

            return new PagedResultDto<XRS_Properties>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }


        public async Task<List<XRS_Properties>> GetPropertiesForApp()
        {
            int customerId = _jwtHelperService.GetCustomerId();

            if (customerId == 0)
                return new List<XRS_Properties>();

            var propertyIds = await _context.UserMapping
                .Where(x => x.userID == customerId && x.isActive == true)
                .Select(x => x.propID)
                .Distinct()
                .ToListAsync();

            if (propertyIds.Count == 0)
                return new List<XRS_Properties>();

            var properties = await _context.Properties
                .Where(p => propertyIds.Contains(p.PropID))
                .Select(p => new XRS_Properties
                {
                    PropID = p.PropID,
                    propertyName = p.propertyName,
                    propertyType = p.propertyType,
                    IsActive = p.IsActive,
                    CompanyId = p.CompanyId
                })
                .ToListAsync();

            return properties;
        }



        public async Task<IEnumerable<XRS_Properties>> GetPrpoertiesbyId(int propertyId)
        {

            return await _context.Properties
                .Where(u => u.PropID == propertyId)
                 .ToListAsync();

        }

        public async Task<XRS_Properties> CreateProperties(XRS_Properties dtoProperties)
        {

            var properties = new XRS_Properties
            {
                propertyName = dtoProperties.propertyName,
                propertyType = dtoProperties.propertyType,
                CompanyId =   dtoProperties.CompanyId,
                IsActive = dtoProperties.IsActive

            };

            await _context.Properties.AddAsync(properties);
            await _context.SaveChangesAsync();
            return properties;

        }

        public async Task<IEnumerable<PropertyWithUnitsDto>> GetPropertyForApp()
        {
            int tenantId = _jwtHelperService.GetCustomerId();


            var assignedUnitIds = await _context.TenantAssignemnts
                .Where(t => t.tenantID == tenantId && t.isActive == true)
                .Select(t => t.unitID)
                .ToListAsync();

            if (!assignedUnitIds.Any())
                return new List<PropertyWithUnitsDto>();

            var result = await (
                from p in _context.Properties
                join u in _context.Units on p.PropID equals u.PropID
                where assignedUnitIds.Contains(u.UnitId)
                group u by new { p.PropID, p.propertyName } into g
                select new PropertyWithUnitsDto
                {
                    PropID = g.Key.PropID,
                    PropertyName = g.Key.propertyName,
                    Units = g.Select(u => new UnitPropertyDto
                    {
                        UnitID = u.UnitId,
                        UnitName = u.UnitName
                    }).ToList()
                }
            ).ToListAsync();

            return result;
        }



        public async Task<bool> UpDateProperties(int id, XRS_Properties properties)
        {
            var updateProperties = await _context.Properties.FirstOrDefaultAsync(u => u.PropID == id);
            if (updateProperties == null) return false;

            updateProperties.propertyName = properties.propertyName;
            updateProperties.propertyType = properties.propertyType;
            updateProperties.CompanyId = properties.CompanyId;
            updateProperties.IsActive = properties.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProperty(int id)
        {
            var bedspacesettings = await _context.Properties.FirstOrDefaultAsync(u => u.PropID == id);
            if (bedspacesettings == null) return false;
            bedspacesettings.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
