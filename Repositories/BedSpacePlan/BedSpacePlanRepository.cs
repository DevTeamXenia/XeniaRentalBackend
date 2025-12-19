using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.BedSpacePlan
{
    public class BedSpacePlanRepository:IBedSpacePlanRepository
    {
        private readonly ApplicationDbContext _context;
        public BedSpacePlanRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<object>> GetAllAsync()
        {
            var result = await _context.BedSpacePlans
                .Select(plan => new
                {
                    plan.bedPlanID,
                    plan.companyID,
                    plan.planName,
                    plan.enableTax,
                    plan.calculatedays,
                    plan.enableAdjustRent,
                    plan.calculateAdjustRent,
                    plan.calculatePartialRent,
                    plan.enableMess,
                    plan.messCharge,
                    plan.includeMess,
                    plan.messChargeDays,
                    plan.consumedDays,
                    plan.isActive
                })
                .ToListAsync();

            return result;
        }

        public async Task<object?> GetByIdAsync(int bedPlanID)
        {
            var resultList = await (from plan in _context.BedSpacePlans
                                    where plan.bedPlanID == bedPlanID
                                    join mapping in _context.BedspacePlanMessMappings
                                        on plan.bedPlanID equals mapping.bedPlanID into pm
                                    from mapping in pm.DefaultIfEmpty()
                                    join mess in _context.MessTypes
                                        on mapping.messID equals mess.messID into mm
                                    from mess in mm.DefaultIfEmpty()
                                    select new
                                    {
                                        plan.bedPlanID,
                                        plan.companyID,
                                        plan.planName,
                                        plan.enableTax,
                                        plan.calculatedays,
                                        plan.enableAdjustRent,
                                        plan.calculateAdjustRent,
                                        plan.calculatePartialRent,
                                        plan.enableMess,
                                        plan.messCharge,
                                        plan.includeMess,
                                        plan.messChargeDays,
                                        plan.consumedDays,
                                        plan.isActive,
                                        Mess = mapping != null
                                            ? new
                                            {
                                                mapping.bpmID,
                                                mapping.messID,
                                                messTypeName = mess != null ? mess.MessName : string.Empty,
                                                mapping.active
                                            }
                                            : null
                                    }).ToListAsync();

            var result = resultList
                .GroupBy(r => new
                {
                    r.bedPlanID,
                    r.companyID,
                    r.planName,
                    r.enableTax,
                    r.calculatedays,
                    r.enableAdjustRent,
                    r.calculateAdjustRent,
                    r.calculatePartialRent,
                    r.enableMess,
                    r.messCharge,
                    r.includeMess,
                    r.messChargeDays,
                    r.consumedDays,
                    r.isActive
                })
                .Select(g => new
                {
                    g.Key.bedPlanID,
                    g.Key.companyID,
                    g.Key.planName,
                    g.Key.enableTax,
                    g.Key.calculatedays,
                    g.Key.enableAdjustRent,
                    g.Key.calculateAdjustRent,
                    g.Key.calculatePartialRent,
                    g.Key.enableMess,
                    g.Key.messCharge,
                    g.Key.includeMess,
                    g.Key.messChargeDays,
                    g.Key.consumedDays,
                    g.Key.isActive,
                    Mess = g.Where(x => x.Mess != null).Select(x => x.Mess).ToList()
                })
                .FirstOrDefault();

            return result;
        }

        public async Task<IEnumerable<object>> GetByCompanyIdAsync(int companyID)
        {
            var result = await (from plan in _context.BedSpacePlans
                                where plan.companyID == companyID
                                join mapping in _context.BedspacePlanMessMappings
                                    on plan.bedPlanID equals mapping.bedPlanID into pm
                                from mapping in pm.DefaultIfEmpty()
                                join mess in _context.MessTypes
                                    on mapping.messID equals mess.messID into mm
                                from mess in mm.DefaultIfEmpty()
                                select new
                                {
                                    plan.bedPlanID,
                                    plan.companyID,
                                    plan.planName,
                                    plan.enableTax,
                                    plan.calculatedays,
                                    plan.enableAdjustRent,
                                    plan.calculateAdjustRent,
                                    plan.calculatePartialRent,
                                    plan.enableMess,
                                    plan.messCharge,
                                    plan.includeMess,
                                    plan.messChargeDays,
                                    plan.consumedDays,
                                    plan.isActive,
                                    Mess = mapping != null
                                        ? new
                                        {
                                            mapping.bpmID,
                                            mapping.messID,
                                            messTypeName = mess != null ? mess.MessName : string.Empty,
                                            mapping.active
                                        }
                                        : null
                                })
                                .ToListAsync();
            var grouped = result
                .GroupBy(r => new { r.bedPlanID, r.planName, r.companyID, r.enableTax, r.calculatedays, r.enableAdjustRent, r.calculateAdjustRent, r.calculatePartialRent, r.enableMess, r.messCharge, r.includeMess, r.messChargeDays, r.consumedDays, r.isActive })
                .Select(g => new
                {
                    g.Key.bedPlanID,
                    g.Key.companyID,
                    g.Key.planName,
                    g.Key.enableTax,
                    g.Key.calculatedays,
                    g.Key.enableAdjustRent,
                    g.Key.calculateAdjustRent,
                    g.Key.calculatePartialRent,
                    g.Key.enableMess,
                    g.Key.messCharge,
                    g.Key.includeMess,
                    g.Key.messChargeDays,
                    g.Key.consumedDays,
                    g.Key.isActive,
                    Mess = g.Where(x => x.Mess != null).Select(x => x.Mess).ToList()
                });

            return grouped;
        }

        public async Task<XRS_BedSpacePlan> CreateAsync(XRS_BedSpacePlan entity)
        {
            _context.BedSpacePlans.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(XRS_BedSpacePlan entity)
        {
            var existing = await _context.BedSpacePlans
                .Include(p => p.BedspacePlanMessMappings) 
                .FirstOrDefaultAsync(p => p.bedPlanID == entity.bedPlanID);

            if (existing == null) return false;
    
            _context.Entry(existing).CurrentValues.SetValues(entity);

            _context.BedspacePlanMessMappings.RemoveRange(existing.BedspacePlanMessMappings);

            if (entity.BedspacePlanMessMappings != null && entity.BedspacePlanMessMappings.Any())
            {
                foreach (var mapping in entity.BedspacePlanMessMappings)
                {
                    existing.BedspacePlanMessMappings.Add(new XRS_BedspacePlanMessMapping
                    {
                        bedPlanID = entity.bedPlanID,
                        messID = mapping.messID,  
                        active = mapping.active
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(int bedID)
        {
            var entity = await _context.BedSpacePlans.FindAsync(bedID);
            if (entity == null) return false;

            _context.BedSpacePlans.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
