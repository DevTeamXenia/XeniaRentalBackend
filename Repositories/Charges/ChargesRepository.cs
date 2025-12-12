using Microsoft.EntityFrameworkCore;
using XeniaRentalApi.Dtos;
using XeniaRentalApi.Models;

namespace XeniaRentalApi.Repositories.Charges
{
    public class ChargesRepository:IChargesRepository
    {
        private readonly ApplicationDbContext _context;
        public ChargesRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<ChargesDto>> GetCharges(int companyId, int? propertyId = null)
        {
            var query = _context.Charges
                .AsNoTracking()
                .Where(c => c.companyID == companyId);

            if (propertyId.HasValue)
            {
                query = query.Where(c => c.PropID == propertyId.Value); 
            }

            return await query
                .Select(c => new ChargesDto
                {
                    chargeID = c.chargeID,
                    chargeName = c.chargeName,
                    PropID = c.PropID,
                    companyID = c.companyID,
                    chargeAmt = c.chargeAmt,
                    isVariable = c.isVariable,
                    isActive = c.isActive,
                    PropName = c.Property != null ? c.Property.propertyName : null
                })
                .ToListAsync();
        }

        public async Task<PagedResultDto<ChargesDto>> GetChargesByCompanyId(int companyId,int? propertyId = null,string? search = null,int pageNumber = 1,int pageSize = 10)
        {

            var query = _context.Charges
                .Where(c => c.companyID == companyId)
                .AsQueryable();

            if (propertyId.HasValue)
            {
                query = query.Where(c => c.PropID == propertyId.Value);
            }

   
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(c =>
                    c.chargeName.ToLower().Contains(lowerSearch)
                );
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .GroupJoin(
                    _context.Properties,
                    charges => charges.PropID,
                    prop => prop.PropID,
                    (charges, props) => new { charges, prop = props.FirstOrDefault() }
                )
                .OrderBy(x => x.charges.chargeName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ChargesDto
                {
                    chargeID = u.charges.chargeID,
                    chargeName = u.charges.chargeName,
                    PropID = u.charges.PropID,
                    companyID = u.charges.companyID,
                    chargeAmt = u.charges.chargeAmt,
                    isActive = u.charges.isActive,
                    isVariable = u.charges.isVariable,
                    PropName = u.prop != null ? u.prop.propertyName : null
                })
                .ToListAsync();

            return new PagedResultDto<ChargesDto>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<ChargesDto?> GetChargesById(int chargeId)
        {
            return await _context.Charges
                .Where(c => c.chargeID == chargeId)
                .GroupJoin(
                    _context.Properties,
                    charges => charges.PropID,
                    prop => prop.PropID,
                    (charges, props) => new { charges, prop = props.FirstOrDefault() }
                )
                .Select(u => new ChargesDto
                {
                    chargeID = u.charges.chargeID,
                    chargeName = u.charges.chargeName,
                    PropID = u.charges.PropID,
                    companyID = u.charges.companyID,
                    chargeAmt = u.charges.chargeAmt,
                    isActive = u.charges.isActive,
                    isVariable = u.charges.isVariable,
                    PropName = u.prop != null ? u.prop.propertyName : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<XRS_Charges> CreateCharges(ChargesDto createCharges)
        {

            var charge = new Models.XRS_Charges
            {
                chargeName= createCharges.chargeName,
                chargeAmt = createCharges.chargeAmt,
                PropID = createCharges.PropID,
                companyID = createCharges.companyID,
                isActive = createCharges.isActive,
                isVariable = createCharges.isVariable

            };
            await _context.Charges.AddAsync(charge);
            await _context.SaveChangesAsync();
            return charge;

        }

        public async Task<bool> DeleteCharges(int id)
        {
            var charger = await _context.Charges.FirstOrDefaultAsync(u => u.chargeID == id);
            if (charger == null) return false;
            charger.isActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCharges(int id, XRS_Charges charges)
        {
            var updatedCharges = await _context.Charges.FirstOrDefaultAsync(u => u.chargeID == id);
            if (updatedCharges == null) return false;

            updatedCharges.chargeName = charges.chargeName ?? charges.chargeName;
            updatedCharges.companyID = charges.companyID;
            updatedCharges.PropID = charges.PropID;
            updatedCharges.isVariable = charges.isVariable;
            updatedCharges.chargeAmt = charges.chargeAmt;
            updatedCharges.isActive = charges.isActive;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
