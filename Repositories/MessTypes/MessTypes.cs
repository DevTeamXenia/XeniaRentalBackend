using Microsoft.EntityFrameworkCore;
using XeniaRentalApi.Dtos;
using XeniaRentalApi.Models;

namespace XeniaRentalApi.Repositories.MessTypes
{
    public class MessTypes:IMessTypes
    {
        private readonly ApplicationDbContext _context;
        public MessTypes(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_Messtypes>> GetMessTypes(int companyId)
        {
            return await _context.MessTypes
                                 .Where(m => m.CompanyId == companyId)
                                 .ToListAsync();
        }

        public async Task<PagedResultDto<XRS_Messtypes>> GetMessTypesByCompanyId(int companyId, int pageNumber, int pageSize)
        {

            var query = _context.MessTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(companyId.ToString()))
            {
                query = query.Where(u => u.CompanyId == companyId);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
            .OrderBy(u => u.MessName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new XRS_Messtypes
            {
              messID = u.messID,
              MessName=u.MessName,
              MessCode=u.MessCode,
              CompanyId=u.CompanyId,
              IsActive=u.IsActive,
               
            })
            .ToListAsync();

            return new PagedResultDto<XRS_Messtypes>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<IEnumerable<XRS_Messtypes>> GetMessTypesbyId(int messTypeId)
        {

            return await _context.MessTypes
                .Where(u => u.messID == messTypeId)
                 .ToListAsync();

        }

        public async Task<XRS_Messtypes> CreateMessTypes(XRS_Messtypes messTypes)
        {

            var messTypesInsert = new XRS_Messtypes
            {
                MessName = messTypes.MessName,
                MessCode = messTypes.MessCode,
                IsActive = messTypes.IsActive,
                CompanyId = messTypes.CompanyId,

            };

            await _context.MessTypes.AddAsync(messTypesInsert);
            await _context.SaveChangesAsync();
            return messTypesInsert;

        }

        public async Task<bool> UpdatMessTypes(int id, XRS_Messtypes types)
        {
            var updatedMessTypes = await _context.MessTypes.FirstOrDefaultAsync(u => u.messID == id);
            if (updatedMessTypes == null) return false;

            updatedMessTypes.MessName = types.MessName;
            updatedMessTypes.messID = types.messID;
            updatedMessTypes.CompanyId = types.CompanyId;
            updatedMessTypes.IsActive = types.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessType(int id)
        {
            var bedspacesettings = await _context.Documents.FirstOrDefaultAsync(u => u.companyID == id);
            if (bedspacesettings == null) return false;
            bedspacesettings.isActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

 
    }
}
