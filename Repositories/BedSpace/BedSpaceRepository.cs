using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.BedSpace
{
    public class BedSpaceRepository: IBedSpaceRepository
    {
        private readonly ApplicationDbContext _context;
        public BedSpaceRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_Bedspace>> GetBedSpaces(int companyId, int? unitId)
        {
            return await _context.BedSpaces
                .AsNoTracking() 
                .Where(b => b.companyID == companyId && b.unitID == unitId)
                .ToListAsync();
        }


        public async Task<PagedResultDto<XRS_Bedspace>> GetBedSpacesByCompanyId(int companyId, string? search = null,int pageNumber = 1,int pageSize = 10)
        {
            var query = _context.BedSpaces
                .Where(b => b.companyID == companyId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(b => b.bedSpaceName.ToLower().Contains(lowerSearch));
            }


            var totalRecords = await query.CountAsync();


            var items = await query
                .OrderBy(b => b.bedSpaceName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new XRS_Bedspace
                {
                    bedID = b.bedID,
                    bedSpaceName = b.bedSpaceName,
                    companyID = b.companyID,
                    planID = b.planID,
                    propID = b.propID,
                    unitID = b.unitID,
                    rentAmt = b.rentAmt,
                    isActive = b.isActive,
                    PropName = b.Properties != null ? b.Properties.propertyName : null,
                    UnitName = b.Units != null ? b.Units.UnitName : null
                })
                .ToListAsync();

            return new PagedResultDto<XRS_Bedspace>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }


        public async Task<XRS_Bedspace> GetBedSpaceById(int bedSpaceId)
        {
            var bedSpace = await _context.BedSpaces
                .Where(b => b.bedID == bedSpaceId)
                .Select(b => new XRS_Bedspace
                {
                    bedID = b.bedID,
                    bedSpaceName = b.bedSpaceName,
                    companyID = b.companyID,
                    planID = b.planID,
                    propID = b.propID,
                    unitID = b.unitID,
                    rentAmt = b.rentAmt,
                    isActive = b.isActive,
                    PropName = b.Properties != null ? b.Properties.propertyName : null,
                    UnitName = b.Units != null ? b.Units.UnitName : null
                })
                .FirstOrDefaultAsync();

            return bedSpace;
        }

  
        public async Task<XRS_Bedspace> CreateBedSpaces(BedSpaceDto dtoBedSpace)
        {

            var bedSpace = new XRS_Bedspace
            {
               companyID = dtoBedSpace.companyID,
               propID = dtoBedSpace.propID,
               unitID = dtoBedSpace.unitID,
               planID = dtoBedSpace.planID,
               bedSpaceName = dtoBedSpace.bedSpaceName,
               rentAmt = dtoBedSpace.rentAmt,
               isActive=dtoBedSpace.isActive

            };
            await _context.BedSpaces.AddAsync(bedSpace);
            await _context.SaveChangesAsync();
            return bedSpace;

        }

        public async Task<bool> UpdateBedSpace(int id, BedSpaceDto bedSpace)
        {
            var updatebedSpace = await _context.BedSpaces.FirstOrDefaultAsync(u => u.bedID == id);
            if (updatebedSpace == null) return false;

            updatebedSpace.bedSpaceName = bedSpace.bedSpaceName ?? bedSpace.bedSpaceName;
            updatebedSpace.companyID = bedSpace.companyID;
            updatebedSpace.planID = bedSpace.planID;
            updatebedSpace.propID = bedSpace.propID;
            updatebedSpace.unitID = bedSpace.unitID;
            updatebedSpace.rentAmt = bedSpace.rentAmt;
            updatebedSpace.isActive = bedSpace.isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBedSpace(int id)
        {
            var bedspacesettings = await _context.BedSpaces.FirstOrDefaultAsync(u => u.bedID == id);
            if (bedspacesettings == null) return false;
            bedspacesettings.isActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
