using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaTenoraBackend.DTOs;


namespace XeniaRentalBackend.Repositories.Company
{
    public class CompanyRepository: ICompanyRepository
    {

        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        //public async Task<CompanyWithSubscriptionDto?> GetCompanyWithSubscriptionAsync(int companyId)
        //{
        //    DateTime currentDate = DateTime.Now;

        //    var company = await _context.Company
        //        .Where(c => c.companyID == companyId)
        //        .Select(c => new CompanyDto
        //        {
        //            companyID = c.companyID,
        //            companyName = c.companyName,
        //            address = c.address,
        //            email = c.email,
        //            phoneNumber = c.phoneNumber,
        //            pin = c.pin,
        //            logo = c.logo,
        //            IsActive = c.IsActive,
        //        })
        //        .FirstOrDefaultAsync();

        //    if (company == null) return null;


        //    var subscription = await _context.CompanySubscription
        //        .Where(s => s.CompanyId == companyId && s.Status != "PENDING")
        //        .OrderByDescending(s => s.SubscriptionDate)
        //        .FirstOrDefaultAsync();

        //    SubscriptionDto? subscriptionDto = null;
        //    PlanDto? planDto = null; 

        //    if (subscription != null)
        //    {
        //        string status = subscription.Status.Trim().ToUpper();

        //        if (status == "ACTIVE" && subscription.SubscriptionEndDate < currentDate)
        //            status = "EXPIRED";
        //        else if (status == "TRIAL" && subscription.SubscriptionEndDate < currentDate)
        //            status = "TRIAL EXPIRED";

        //        subscriptionDto = new SubscriptionDto
        //        {
        //            SubId = subscription.SubId,
        //            PlanId = subscription.PlanId,
        //            SubscriptionStartDate = subscription.SubscriptionStartDate,
        //            SubscriptionEndDate = subscription.SubscriptionEndDate,
        //            SubscriptionAmount = subscription.SubscriptionAmount,
        //            SubscriptionDays = subscription.SubscriptionDays,
        //            Status = status
        //        };


        //        var plan = await _context.SubscribePlan
        //            .Where(p => p.PlanId == subscription.PlanId && p.PlanActive == true)
        //            .FirstOrDefaultAsync();

        //        if (plan != null)
        //        {
        //            planDto = new PlanDto
        //            {
        //                PlanId = plan.PlanId,
        //                PlanName = plan.PlanName ?? string.Empty,           // ✅ null coalesce
        //                PlanDescription = plan.PlanDescription ?? string.Empty, // ✅ null coalesce
        //                PlanIsAddOn = plan.PlanIsAddOn,
        //                PlanActive = plan.PlanActive ?? false               // ✅ null coalesce
        //            };


        //            var modules = await (
        //                from pm in _context.PlanModuleMap
        //                join m in _context.Module on pm.ModuleId equals m.ModuleId
        //                where pm.PlanId == plan.PlanId &&
        //                      pm.Active &&
        //                      m.ModuleActive
        //                select new ModuleDto
        //                {
        //                    ModuleId = m.ModuleId,
        //                    ModuleName = m.ModuleName,
        //                    ModuleDescription = m.ModuleDescription,
        //                    ModuleActive = m.ModuleActive
        //                }).ToListAsync();

        //            planDto.Modules = modules;
        //        }
        //    }

        //    return new CompanyWithSubscriptionDto
        //    {
        //        Company = company,
        //        Subscription = subscriptionDto,
        //        Plan = planDto
        //    };

        //    // After planDto block, before return
        //    var settings = await _context.CompanySettings
        //        .Where(s => s.CompanyId == companyId && s.Active)
        //        .Select(s => new CompanySettingsDto
        //        {
        //            CompanySettingsId = s.CompanySettingsId,
        //            CompanyId = s.CompanyId,
        //            KeyCode = s.KeyCode,
        //            Value = s.Value,
        //            Active = s.Active
        //        })
        //        .ToListAsync();

        //    return new CompanyWithSubscriptionDto
        //    {
        //        Company = company,
        //        Subscription = subscriptionDto,
        //        Plan = planDto,
        //        Settings = settings  // ADD THIS
        //    };

        //}
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

            var subscription = await _context.CompanySubscription
                .Where(s => s.CompanyId == companyId && s.Status != "PENDING")
                .OrderByDescending(s => s.SubscriptionDate)
                .FirstOrDefaultAsync();

            SubscriptionDto? subscriptionDto = null;
            PlanDto? planDto = null;

            if (subscription != null)
            {
                string status = subscription.Status.Trim().ToUpper();

                if (status == "ACTIVE" && subscription.SubscriptionEndDate < currentDate)
                    status = "EXPIRED";
                else if (status == "TRIAL" && subscription.SubscriptionEndDate < currentDate)
                    status = "TRIAL EXPIRED";

                subscriptionDto = new SubscriptionDto
                {
                    SubId = subscription.SubId,
                    PlanId = subscription.PlanId,
                    SubscriptionStartDate = subscription.SubscriptionStartDate,
                    SubscriptionEndDate = subscription.SubscriptionEndDate,
                    SubscriptionAmount = subscription.SubscriptionAmount,
                    SubscriptionDays = subscription.SubscriptionDays,
                    Status = status
                };

                var plan = await _context.SubscribePlan
                    .Where(p => p.PlanId == subscription.PlanId && p.PlanActive == true)
                    .FirstOrDefaultAsync();

                if (plan != null)
                {
                    planDto = new PlanDto
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName ?? string.Empty,
                        PlanDescription = plan.PlanDescription ?? string.Empty,
                        PlanIsAddOn = plan.PlanIsAddOn,
                        PlanActive = plan.PlanActive ?? false
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
            }


            var settings = await _context.CompanySettings
                .Where(s => s.CompanyId == companyId && s.Active)
                .Select(s => new CompanySettingsDto
                {
                    CompanySettingsId = s.CompanySettingsId,
                    CompanyId = s.CompanyId,
                    KeyCode = s.KeyCode,
                    Value = s.Value,
                    Active = s.Active
                })
                .ToListAsync();

            var userCount = await _context.Users
                .CountAsync(u => u.CompanyId == companyId && u.IsActive);

            return new CompanyWithSubscriptionDto
            {
                Company = company,
                Subscription = subscriptionDto,
                Plan = planDto,
                Settings = settings,
                UserCount = userCount
            };
        }

        public async Task<bool> UpdateCompany(int id, XRS_Company company)
        {
            var updatedCompany = await _context.Company
                .FirstOrDefaultAsync(u => u.companyID == id);

            if (updatedCompany == null) return false;

            updatedCompany.companyName = company.companyName;
            updatedCompany.phoneNumber = company.phoneNumber;
            updatedCompany.address = company.address;
            updatedCompany.pin = company.pin;
            updatedCompany.email = company.email;
            updatedCompany.logo = company.logo;
            updatedCompany.IsActive = company.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
