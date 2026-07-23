using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;


namespace XeniaRentalBackend.Repositories.Company
{
    public class CompanyRepository: ICompanyRepository
    {

        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<CompanyWithSubscriptionDto?> GetCompanyWithSubscriptionAsync(int companyId)
        {
            DateTime currentDate = DateTime.Now;

            var company = await _context.Company
                .Where(c => c.companyID == companyId)
                .Select(c => new CompanyDto
                {
                    companyID = c.companyID,
                    companyName = c.companyName,
                    address = c.address,
                    email = c.email,
                    phoneNumber = c.phoneNumber,
                    pin = c.pin,
                    logo = c.logo,
                    IsActive = c.IsActive,
                })
                .FirstOrDefaultAsync();

            if (company == null) return null;

       
            var companySettings = await _context.CompanySettings
                .Where(cs => cs.CompanyId == companyId)
                .Select(cs => new CompanySettingDto
                {
                    Id = cs.CompanySettingsId,
                    KeyCode = cs.KeyCode,
                    Value = cs.Value
                })
                .ToListAsync();

            var subscription = await _context.CompanySubscriptions
                .Where(s => s.CompanyId == companyId && s.Status != "PENDING")
                .OrderByDescending(s => s.SubscriptionEndDate)
                .ThenByDescending(s => s.SubscriptionStartDate)
                .FirstOrDefaultAsync();

            SubscriptionDto? subscriptionDto = null;
            PlanDto? planDto = null;
            List<SubscriptionAddonDto> addonDtos = new();

            if (subscription != null)
            {
                string status = (subscription.Status ?? "").Trim().ToUpper();

                var endDate = subscription.SubscriptionEndDate;

                if ((status == "ACTIVE" || status == "SUCCESS") &&
                    endDate.HasValue && endDate.Value < currentDate)
                {
                    status = "EXPIRED";
                }
                else if (status == "TRIAL" &&
                         endDate.HasValue && endDate.Value < currentDate)
                {
                    status = "TRIAL_EXPIRED";
                }
                else if (status == "SUCCESS")
                {
                    status = "ACTIVE";
                }

                int? expireDays = endDate.HasValue
                    ? Math.Max((endDate.Value.Date - currentDate.Date).Days, 0)
                    : (int?)null;

                subscriptionDto = new SubscriptionDto
                {
                    SubId = subscription.SubId,
                    PlanId = subscription.PlanId,
                    SubscriptionStartDate = subscription.SubscriptionStartDate,
                    SubscriptionEndDate = subscription.SubscriptionEndDate,
                    SubscriptionAmount = subscription.SubscriptionAmount,
                    SubscriptionDays = subscription.SubscriptionDays,
                    SubscriptionUserCount = subscription.SubscriptionUserCount,
                    RateType = subscription.RateType,
                    ExpireDays = expireDays,
                    Status = status
                };

             
                var plan = await _context.SubscribePlan
                    .Where(p => p.PlanId == subscription.PlanId && p.PlanActive)
                    .FirstOrDefaultAsync();

                if (plan != null)
                {
                    planDto = new PlanDto
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName ?? string.Empty,
                        PlanDescription = plan.PlanDescription,
                        PlanPrice = plan.PlanPrice,
                        PlanDurationDays = subscription.SubscriptionDays,
                        PlanIsAddOn = plan.PlanIsAddOn,
                        PlanActive = plan.PlanActive
                    };

                    var modules = await (
                        from pm in _context.PlanModuleMap
                        join m in _context.Module on pm.ModuleId equals m.ModuleId
                        where pm.PlanId == plan.PlanId &&
                              pm.Active &&
                              m.ModuleActive
                        select new ModuleDto
                        {
                            ModuleId = m.ModuleId,
                            ModuleName = m.ModuleName,
                            ModuleDescription = m.ModuleDescription,
                            ModuleActive = m.ModuleActive
                        }).ToListAsync();

                    planDto.Modules = modules;
                }

             
                if (status == "ACTIVE" || status == "TRIAL")
                {
                    addonDtos = await (
                        from a in _context.CompanySubscriptionAddon
                        join p in _context.SubscribePlan on a.PlanId equals p.PlanId into planJoin
                        from p in planJoin.DefaultIfEmpty()
                        where a.CompanyId == companyId &&
                              a.MainPlanId == subscription.SubId &&
                              (a.Status == "SUCCESS" || a.Status == "ACTIVE")
                        select new SubscriptionAddonDto
                        {
                            Id = a.Id,
                            PlanId = a.PlanId,
                            PlanName = p != null ? p.PlanName : null,
                            Amount = a.Amount,
                            DealerAmount = a.DealerAmount,
                            RateType = a.RateType,
                            UserCount = a.UserCount,
                            Status = a.Status
                        }).ToListAsync();
                }
            }

            return new CompanyWithSubscriptionDto
            {
                Company = company,
                Subscription = subscriptionDto,
                Plan = planDto,
                Addons = addonDtos,
                CompanySettings = companySettings
            };
        }


        public async Task<XRS_Company> UpdateCompany(int id, CompanySettingUpdateDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var updatedCompany = await _context.Company
                    .FirstOrDefaultAsync(v => v.companyID == id);

                if (updatedCompany == null)
                    throw new Exception("Voucher not found");

                updatedCompany.companyName = request.companyName;
                updatedCompany.phoneNumber = request.phoneNumber;
                updatedCompany.address = request.address;
                updatedCompany.pin = request.pin;
                updatedCompany.email = request.email;
                updatedCompany.logo = request.logo;
                updatedCompany.IsActive = request.IsActive;

                var existingDetails = await _context.CompanySettings
                    .Where(d => d.CompanyId == updatedCompany.companyID)
                    .ToListAsync();

                _context.CompanySettings.RemoveRange(existingDetails);

                foreach (var detail in request.CompanyDetails)
                {
                    _context.CompanySettings.Add(new XRS_CompanySettings
                    {
                        CompanyId = updatedCompany.companyID,
                        KeyCode = detail.KeyCode,
                        Value = detail.Value
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return updatedCompany;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
