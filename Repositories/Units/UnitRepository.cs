using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Units;

namespace XeniaRentalBackend.Repositories.Unit
{
    public class UnitRepository : IUnitRepository
    {
        private readonly ApplicationDbContext _context;

        public UnitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UnitDto>> GetUnits(int companyId, int? propertyId = null)
        {
            var query = _context.Units
                .AsNoTracking()
                .Include(u => u.Property)
                .Include(u => u.Category)
                .Where(u => u.CompanyId == companyId && u.IsActive);


            if (propertyId.HasValue)
            {
                query = query.Where(u => u.PropID == propertyId.Value);
            }

 
            var units = await query
                .OrderBy(u => u.UnitName)
                .ToListAsync();

       
            var unitsDto = units.Select(u => new UnitDto
            {
                UnitId = u.UnitId,
                UnitName = u.UnitName,
                PropID = u.PropID,
                CompanyId = u.CompanyId,
                UnitType = u.UnitType,
                PropName = u.Property?.propertyName,
                CatID = u.CatID,
                CategoryName = u.Category?.CategoryName,
                IsActive = u.IsActive,
                Area = u.Area,
                Remarks = u.Remarks,
                FloorNo = u.FloorNo,
                DefaultRent = u.DefaultRent,
                escalationper = u.escalationper,
                UnitCharges = null 
            }).ToList();

            return unitsDto;
        }

        public async Task<PagedResultDto<UnitDto>> GetUnitByCompanyId(int companyId, string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Units
                .Include(u => u.Property)
                .Include(u => u.Category)
                .Where(u => u.CompanyId == companyId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(u =>
                    u.UnitName.ToLower().Contains(lowerSearch) ||
                    (u.Property != null && u.Property.propertyName.ToLower().Contains(lowerSearch)) ||
                    (u.Category != null && u.Category.CategoryName.ToLower().Contains(lowerSearch))
                );
            }

            var totalRecords = await query.CountAsync();

            var unitsInMemory = await query
                .OrderBy(u => u.UnitName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var unitIds = unitsInMemory.Select(u => u.UnitId).ToList();

            var unitChargesDict = await _context.UnitChargesMappings
                .Where(uc => unitIds.Contains(uc.unitID))
                .Include(uc => uc.Charges)
                .ToListAsync();

            var unitsDto = unitsInMemory.Select(u =>
            {
                var charges = unitChargesDict
                    .Where(uc => uc.unitID == u.UnitId)
                    .Select(uc => new UnitChargeDto
                    {
                        unitMapID = uc.unitMapID,
                        chargeID = uc.chargeID,
                        amount = uc.amount,
                        frequency = uc.frequency,
                        isActive = uc.isActive,
                        ChargeName = uc.Charges?.chargeName,
                        ChargeType = uc.Charges != null && uc.Charges.isVariable ? "Variable" : "Fixed"
                    }).ToList();

                return new UnitDto
                {
                    UnitId = u.UnitId,
                    UnitName = u.UnitName,
                    PropID = u.PropID,
                    CompanyId = u.CompanyId,
                    UnitType = u.UnitType,
                    PropName = u.Property?.propertyName,
                    CatID = u.CatID,
                    CategoryName = u.Category?.CategoryName,
                    IsActive = u.IsActive,
                    Area = u.Area,
                    Remarks = u.Remarks,
                    FloorNo = u.FloorNo,
                    DefaultRent = u.DefaultRent,
                    escalationper = u.escalationper,
                    UnitCharges = charges
                };
            }).ToList();

            return new PagedResultDto<UnitDto>
            {
                Data = unitsDto,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<UnitDto?> GetUnitById(int unitId)
        {
            var unit = await _context.Units
                .Include(u => u.Property)
                .Include(u => u.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UnitId == unitId);

            if (unit == null)
                return null;

            var unitCharges = await _context.UnitChargesMappings
                .Where(uc => uc.unitID == unit.UnitId)
                .Include(uc => uc.Charges)
                .ToListAsync();

            var chargesDto = unitCharges.Select(uc => new UnitChargeDto
            {
                unitMapID = uc.unitMapID,
                chargeID = uc.chargeID,
                amount = uc.amount,
                frequency = uc.frequency,
                isActive = uc.isActive,
                ChargeName = uc.Charges?.chargeName,
                ChargeType = uc.Charges != null && uc.Charges.isVariable ? "Variable" : "Fixed"
            }).ToList();

            return new UnitDto
            {
                UnitId = unit.UnitId,
                UnitName = unit.UnitName,
                PropID = unit.PropID,
                CompanyId = unit.CompanyId,
                UnitType = unit.UnitType,
                PropName = unit.Property?.propertyName,
                CatID = unit.CatID,
                CategoryName = unit.Category?.CategoryName,
                IsActive = unit.IsActive,
                Area = unit.Area,
                Remarks = unit.Remarks,
                FloorNo = unit.FloorNo,
                DefaultRent = unit.DefaultRent,
                escalationper = unit.escalationper,
                UnitCharges = chargesDto
            };
        }

        public async Task<UnitDto> CreateUnit(UnitDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var unit = new XRS_Units
            {
                UnitName = model.UnitName,
                PropID = model.PropID,
                CompanyId = model.CompanyId,
                UnitType = model.UnitType,
                CatID = model.CatID,
                IsActive = model.IsActive,
                Area = model.Area,
                Remarks = model.Remarks,
                FloorNo = model.FloorNo,
                DefaultRent = model.DefaultRent,
                escalationper = model.escalationper
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync(); 

            if (model.UnitCharges != null && model.UnitCharges.Any())
            {
                var unitCharges = model.UnitCharges.Select(uc => new XRS_UnitChargesMapping
                {
                    unitID = unit.UnitId,
                    propID = unit.PropID,
                    companyID = unit.CompanyId,
                    chargeID = uc.chargeID,
                    amount = uc.amount,
                    frequency = uc.frequency,
                    isActive = uc.isActive
                }).ToList();

                _context.UnitChargesMappings.AddRange(unitCharges);
                await _context.SaveChangesAsync(); 
            }

            return new UnitDto
            {
                UnitId = unit.UnitId,
                UnitName = unit.UnitName,
                PropID = unit.PropID,
                CompanyId = unit.CompanyId,
                UnitType = unit.UnitType,
                CatID = unit.CatID,
                IsActive = unit.IsActive,
                Area = unit.Area,
                Remarks = unit.Remarks,
                FloorNo = unit.FloorNo,
                DefaultRent = unit.DefaultRent,
                escalationper = unit.escalationper,
                UnitCharges = model.UnitCharges
            };
        }

        public async Task<UnitDto?> UpdateUnit(UnitDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var unit = await _context.Units.FirstOrDefaultAsync(u => u.UnitId == model.UnitId);

            if (unit == null)
                return null;

            var existingCharges = await _context.UnitChargesMappings
                .Where(uc => uc.unitID == unit.UnitId)
                .ToListAsync();

            unit.UnitName = model.UnitName;
            unit.PropID = model.PropID;
            unit.CompanyId = model.CompanyId;
            unit.UnitType = model.UnitType;
            unit.CatID = model.CatID;
            unit.IsActive = model.IsActive;
            unit.Area = model.Area;
            unit.Remarks = model.Remarks;
            unit.FloorNo = model.FloorNo;
            unit.DefaultRent = model.DefaultRent;
            unit.escalationper = model.escalationper;


            if (model.UnitCharges != null)
            {
                var chargesToRemove = existingCharges
                    .Where(uc => !model.UnitCharges.Any(m => m.unitMapID == uc.unitMapID))
                    .ToList();

                if (chargesToRemove.Any())
                    _context.UnitChargesMappings.RemoveRange(chargesToRemove);

                foreach (var ucDto in model.UnitCharges)
                {
                    var existingCharge = existingCharges.FirstOrDefault(uc => uc.unitMapID == ucDto.unitMapID);

                    if (existingCharge != null)
                    {
                        existingCharge.chargeID = ucDto.chargeID;
                        existingCharge.amount = ucDto.amount;
                        existingCharge.frequency = ucDto.frequency;
                        existingCharge.isActive = ucDto.isActive;
                    }
                    else
                    {
                        _context.UnitChargesMappings.Add(new XRS_UnitChargesMapping
                        {
                            unitID = unit.UnitId,
                            propID = unit.PropID,
                            companyID = unit.CompanyId,
                            chargeID = ucDto.chargeID,
                            amount = ucDto.amount,
                            frequency = ucDto.frequency,
                            isActive = ucDto.isActive
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return model; 
        }

        public async Task<bool> DeleteUnit(int unitId)
        {
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.UnitId == unitId);
            if (unit == null) return false;

            unit.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
