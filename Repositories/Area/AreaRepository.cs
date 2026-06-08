using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Models;

namespace XeniaTenoraBackend.Repositories.Area
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<XRS_Area>> GetAllAreas(int companyId)
        {
            return await _context.Areas
                .Where(a => a.Active && a.CompanyID == companyId)
                .ToListAsync();
        }

        public async Task<XRS_Area?> GetAreaById(int id)
        {
            return await _context.Areas
                .FirstOrDefaultAsync(a => a.AreaId == id);
        }

        public async Task<int> AddArea(XRS_Area area)
        {
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area.AreaId;
        }

        public async Task<bool> UpdateArea(XRS_Area area)
        {
            _context.Areas.Update(area);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteArea(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return false;

            area.Active = false;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}