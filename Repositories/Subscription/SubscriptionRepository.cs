using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Payment;
using XeniaTenoraBackend.Models;

namespace XeniaRentalBackend.Repositories.Subscription
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly HttpClient _httpClient;

        public SubscriptionRepository(ApplicationDbContext context, IPaymentService paymentService, HttpClient httpClient)
        {
            _context = context;
            _paymentService = paymentService;
            _httpClient = httpClient;
        }


        public async Task<List<PlanWithModulesDto>> GetMainPlansAsync(int companyId)
        {
            var plans = await _context.SubscribePlan
                .Where(p => p.PlanActive && !p.PlanIsAddOn)
                .Include(p => p.PlanDurations)
                .ToListAsync();


            var specialRates = await _context.SubscriptionSpecialRate
                .Where(r => r.CompanyId == companyId && r.IsActive && r.AddonId == null)
                .ToListAsync();

            var result = new List<PlanWithModulesDto>();

            foreach (var plan in plans)
            {
                var modules = await (
                    from pm in _context.PlanModuleMap
                    join m in _context.Module on pm.ModuleId equals m.ModuleId
                    where pm.PlanId == plan.PlanId
                          && pm.Active
                          && m.ModuleActive
                    select new ModuleDto
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.ModuleName,
                        ModuleDescription = m.ModuleDescription,
                        ModuleActive = m.ModuleActive
                    }).ToListAsync();

                var durations = plan.PlanDurations
                    .Where(d => d.IsActive)
                    .OrderBy(d => d.DurationDays)
                    .Select(d =>
                    {
                        var specialRate = specialRates
                            .FirstOrDefault(r =>
                                r.PlanId == plan.PlanId &&
                                (r.PlanDurationId == null || r.PlanDurationId == d.PlanDurationId));

                        return new PlanDurationDto
                        {
                            PlanDurationId = d.PlanDurationId,
                            DurationDays = d.DurationDays,
                            Price = d.Price,
                            DPrice = d.DealerPrice,
                            CPrice = specialRate != null ? specialRate.CustomRate : d.CustomPrice
                        };
                    }).ToList();

                result.Add(new PlanWithModulesDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName ?? string.Empty,
                    PlanDescription = plan.PlanDescription ?? string.Empty,
                    PlanUsers = plan.PlanUsers ?? 0,
                    Durations = durations,
                    Modules = modules
                });
            }

            return result;
        }


        public async Task<List<AddonPlanDto>> GetAddonPlansAsync(int companyId)
        {
            var addonPlans = await _context.SubscribePlan
                .Where(p => p.PlanActive && p.PlanIsAddOn)
                .ToListAsync();

            var specialRates = await _context.SubscriptionSpecialRate
                .Where(r => r.CompanyId == companyId && r.IsActive && r.AddonId != null)
                .ToListAsync();

            return addonPlans.Select(p =>
            {
                var specialRate = specialRates.FirstOrDefault(r => r.AddonId == p.PlanId);

                return new AddonPlanDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName ?? string.Empty,
                    PlanUsers = p.PlanUsers ?? 0,
                    PlanPrice = p.PlanPrice ?? 0,
                    PlanDPrice = p.PlanDPrice ?? 0,
                    PlanCPrice = specialRate != null ? specialRate.CustomRate : (p.PlanCPrice ?? 0)
                };
            }).ToList();
        }
        public async Task<RenewSubscriptionResponseDto?> RenewSubscriptionAsync(int userId, RenewSubscriptionDto dto)
        {
            var existingTransaction = await _context.SubscriptionTransaction
                .Where(t =>
                    t.CompanyId == dto.CompanyId &&
                    (t.Status == "INITIATED" || t.Status == "PENDING"))
                .OrderByDescending(t => t.CreatedOn)
                .FirstOrDefaultAsync();

            if (existingTransaction != null)
            {

                var gatewayResponse = await _paymentService.CheckSubTransactionStatusAsync(existingTransaction.PaymentRef);

                if (gatewayResponse?.Data == null || gatewayResponse.Data.Count == 0)
                {
                    throw new Exception("Unable to retrieve payment status from gateway.");
                }

                var latestTxn = gatewayResponse.Data.First();

                if (latestTxn.Payment_Status == 1)
                {
                    return new RenewSubscriptionResponseDto
                    {
                        TransactionId = existingTransaction.TransactionRefId,
                        PaymentLink = existingTransaction.PaymentLink,
                        PaymentStatus = latestTxn.Payment_Desc ?? "SUCCESS",
                        Message = "Payment already completed successfully for this subscription."
                    };
                }

                if (latestTxn.Payment_Status == 2)
                {
                    var minutesElapsed = (DateTime.Now - existingTransaction.CreatedOn.GetValueOrDefault()).TotalMinutes;

                    if (minutesElapsed <= 15)
                    {
                        return new RenewSubscriptionResponseDto
                        {
                            TransactionId = existingTransaction.TransactionRefId,
                            PaymentLink = existingTransaction.PaymentLink,
                            PaymentStatus = gatewayResponse.ResponseMessage ?? gatewayResponse.Status,
                            Message = "A payment is already in progress. Please try after sometimes."
                        };
                    }
                    else
                    {
                        return new RenewSubscriptionResponseDto
                        {
                            TransactionId = existingTransaction.TransactionRefId,
                            PaymentLink = existingTransaction.PaymentLink,
                            PaymentStatus = gatewayResponse.ResponseMessage ?? gatewayResponse.Status,
                            Message = "A payment is in pending. Please wait for confirmation."
                        };
                    }
                }
                else if (latestTxn.Payment_Status == 0)
                {
                    await ExpirePendingSubscriptions(
                        existingTransaction.CompanyId,
                        existingTransaction.SubscriptionId);

                    existingTransaction.Status = "FAILED";
                    await _context.SaveChangesAsync();
                }
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var mainPlan = await _context.SubscribePlan
                    .Include(p => p.PlanDurations)
                    .FirstOrDefaultAsync(p =>
                        p.PlanId == dto.PlanId &&
                        p.PlanActive &&
                        !p.PlanIsAddOn);

                if (mainPlan == null) return null;

                var selectedDuration = mainPlan.PlanDurations
                    .FirstOrDefault(d =>
                        d.PlanDurationId == dto.PlanDurationId &&
                        d.IsActive);

                if (selectedDuration == null) return null;

                decimal totalAmount = selectedDuration.CustomPrice;

                var merchantTxnId =
                    $"TXN{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid():N}".Substring(0, 30);

                var addons = new List<XRS_SubscribePlan>();

                if (dto.AddonPlanIds?.Any() == true)
                {
                    addons = await _context.SubscribePlan
                        .Include(p => p.PlanDurations)
                        .Where(p => dto.AddonPlanIds.Contains(p.PlanId) && p.PlanIsAddOn)
                        .ToListAsync();

                    totalAmount += addons.Sum(a =>
                        a.PlanDurations
                            .Where(d => d.IsActive)
                            .Select(d => d.Price)
                            .FirstOrDefault());
                }

                var transaction = new XRS_SubscriptionTransaction
                {
                    CompanyId = dto.CompanyId,
                    Amount = totalAmount,
                    ModifiedOn = DateTime.Now,
                    PaymentProvider = "MSWIPE",
                    TransactionRefId = merchantTxnId,
                    MOP = "ONLINE",
                    Status = "INITIATED",
                    CreatedOn = DateTime.Now,
                    SubscriptionId = null
                };

                _context.SubscriptionTransaction.Add(transaction);
                await _context.SaveChangesAsync();

                try
                {
                    string paymentLink = await _paymentService.CreateSubPaymentLink(
                        merchantTxnId,
                        totalAmount);

                    string? transId = null;

                    try
                    {
                        var uri = new Uri(paymentLink);
                        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        transId = queryParams["TransID"];
                    }
                    catch
                    {
                        transId = paymentLink.Contains("TransID=")
                            ? paymentLink.Split("TransID=").LastOrDefault()
                            : null;
                    }

                    transaction.PaymentLink = paymentLink;
                    transaction.PaymentRef = transId;
                    transaction.TransactionRefId = merchantTxnId;
                    transaction.Status = "PENDING";
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    transaction.PaymentLink = "";
                    transaction.TransactionRefId = "";
                    transaction.Status = "FAILED";
                    await _context.SaveChangesAsync();
                    await tx.RollbackAsync();
                    throw new Exception("MSWIPE PAYMENT ERROR : " + ex.Message);
                }


                var activeSubscription = await _context.CompanySubscriptions
                .Where(s =>
                    s.CompanyId == dto.CompanyId &&
                    s.Status == "SUCCESS" &&
                    s.SubscriptionEndDate > DateTime.Now)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

                var startDate = activeSubscription != null
                    ? activeSubscription.SubscriptionEndDate!.Value
                    : DateTime.Now;

                var endDate = startDate.AddDays(selectedDuration.DurationDays);

                var subscription = new XRS_CompanySubscription
                {
                    CompanyId = dto.CompanyId,
                    PlanId = mainPlan.PlanId,
                    PlanDurationId = selectedDuration.PlanDurationId,
                    SubscriptionStartDate = startDate,
                    SubscriptionEndDate = endDate,
                    SubscriptionAmount = selectedDuration.Price,
                    SubscriptionDays = selectedDuration.DurationDays,
                    SubscriptionUserCount = mainPlan.PlanUsers,
                    //RateType = "CUSTOMER DIRECT",
                    Status = "PENDING",
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now
                };

                _context.CompanySubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                transaction.SubscriptionId = subscription.SubId;
                await _context.SaveChangesAsync();

                foreach (var addon in addons)
                {
                    var addonPrice = addon.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => d.Price)
                        .FirstOrDefault();

                    _context.CompanySubscriptionAddon.Add(new XRS_CompanySubscriptionAddon
                    {
                        CompanyId = dto.CompanyId,
                        MainPlanId = subscription.SubId,
                        PlanId = addon.PlanId,
                        Amount = addonPrice,
                        UserCount = addon.PlanUsers,
                        Status = "PENDING",
                        CreatedOn = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new RenewSubscriptionResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    PaymentLink = transaction.PaymentLink,
                    PaymentStatus = transaction.Status,
                    Message = string.IsNullOrEmpty(transaction.PaymentLink)
                        ? "Subscription created. Payment gateway temporarily unavailable."
                        : "Payment link generated successfully"
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task ExpirePendingSubscriptions(int companyId, int? subId)
        {
            var subscription = await _context.CompanySubscriptions
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == companyId &&
                    x.SubId == subId);

            if (subscription != null)
            {
                subscription.Status = "FAILED";
            }

            var pendingAddons = await _context.CompanySubscriptionAddon
                .Where(x => x.CompanyId == companyId && x.Status == "PENDING")
                .ToListAsync();

            foreach (var addon in pendingAddons)
            {
                addon.Status = "FAILED";
            }

            await _context.SaveChangesAsync();
        }
        public async Task<CancelSubscriptionResponseDto?> CancelPendingSubscriptionAsync(int userId, int companyId)
        {
            var pendingTransactions = await _context.SubscriptionTransaction
                .Where(t =>
                    t.CompanyId == companyId &&
                    (t.Status == "INITIATED" || t.Status == "PENDING"))
                .ToListAsync();

            if (!pendingTransactions.Any())
            {
                return new CancelSubscriptionResponseDto
                {
                    Cancelled = false,
                    Message = "No pending or initiated transactions found to cancel."
                };
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var transaction in pendingTransactions)
                {
                    transaction.Status = "USERCANCELLED";
                    transaction.ModifiedOn = DateTime.Now;

                    if (transaction.SubscriptionId.HasValue)
                    {
                        var subscription = await _context.CompanySubscriptions
                            .FirstOrDefaultAsync(s => s.SubId == transaction.SubscriptionId.Value);

                        if (subscription != null)
                        {
                            subscription.Status = "USERCANCELLED";
                        }

                        var addons = await _context.CompanySubscriptionAddon
                            .Where(a => a.MainPlanId == transaction.SubscriptionId.Value)
                            .ToListAsync();

                        foreach (var addon in addons)
                        {
                            addon.Status = "USERCANCELLED";
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new CancelSubscriptionResponseDto
                {
                    Cancelled = true,
                    Message = "All pending/initiated transactions have been cancelled."
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<AddAddonResponseDto?> AddAddonToSubscriptionAsync(int userId, AddAddonDto dto)
        {
            var existingTransaction = await _context.SubscriptionTransaction
                .Where(t =>
                    t.CompanyId == dto.CompanyId &&
                    (t.Status == "INITIATED" || t.Status == "PENDING"))
                .OrderByDescending(t => t.CreatedOn)
                .FirstOrDefaultAsync();

            if (existingTransaction != null)
            {
                var gatewayResponse = await _paymentService.CheckSubTransactionStatusAsync(existingTransaction.PaymentRef);

                if (gatewayResponse.ResponseMessage == "Transaction detail not found")
                {
                    var minutesElapsed = (DateTime.Now - existingTransaction.CreatedOn.GetValueOrDefault()).TotalMinutes;

                    if (minutesElapsed <= 15)
                    {
                        return new AddAddonResponseDto
                        {
                            TransactionId = existingTransaction.TransactionRefId,
                            PaymentLink = existingTransaction.PaymentLink,
                            PaymentStatus = gatewayResponse.ResponseMessage ?? gatewayResponse.Status,
                            Message = "A payment is already in progress. Please try after sometimes."
                        };
                    }
                    else
                    {
                        return new AddAddonResponseDto
                        {
                            TransactionId = existingTransaction.TransactionRefId,
                            PaymentLink = existingTransaction.PaymentLink,
                            PaymentStatus = gatewayResponse.ResponseMessage ?? gatewayResponse.Status,
                            Message = "A payment is already generated. Please complete the existing transaction."
                        };
                    }
                }
                else if (gatewayResponse.Status == "PENDING")
                {
                    return new AddAddonResponseDto
                    {
                        TransactionId = existingTransaction.TransactionRefId,
                        PaymentLink = existingTransaction.PaymentLink,
                        PaymentStatus = gatewayResponse.ResponseMessage ?? gatewayResponse.Status,
                        Message = "A payment is in pending. Please wait for confirmation."
                    };
                }
                else if (gatewayResponse.Status == "FAILED")
                {
                    await ExpirePendingSubscriptions(
                        existingTransaction.CompanyId,
                        existingTransaction.SubscriptionId);

                    existingTransaction.Status = "FAILED";
                    await _context.SaveChangesAsync();
                }
            }

            var activeSubscription = await _context.CompanySubscriptions
                .Where(s =>
                    s.CompanyId == dto.CompanyId &&
                    s.Status == "SUCCESS" &&
                    s.SubscriptionEndDate > DateTime.Now)
                .OrderByDescending(s => s.SubscriptionEndDate)
                .FirstOrDefaultAsync();

            if (activeSubscription == null)
            {
                return new AddAddonResponseDto
                {
                    Message = "No active subscription found. Please renew your subscription first."
                };
            }

            if (dto.AddonPlanIds == null || !dto.AddonPlanIds.Any())
            {
                return new AddAddonResponseDto
                {
                    Message = "Please select at least one addon."
                };
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var addons = await _context.SubscribePlan
                    .Include(p => p.PlanDurations)
                    .Where(p => dto.AddonPlanIds.Contains(p.PlanId) && p.PlanIsAddOn && p.PlanActive)
                    .ToListAsync();

                if (!addons.Any())
                {
                    await tx.RollbackAsync();
                    return null;
                }

                decimal totalAmount = addons.Sum(a =>
                    a.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => d.Price)
                        .FirstOrDefault());

                var merchantTxnId =
                    $"TXN{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid():N}".Substring(0, 30);

                var transaction = new XRS_SubscriptionTransaction
                {
                    CompanyId = dto.CompanyId,
                    Amount = totalAmount,
                    ModifiedOn = DateTime.Now,
                    PaymentProvider = "MSWIPE",
                    TransactionRefId = merchantTxnId,
                    MOP = "ONLINE",
                    Status = "INITIATED",
                    CreatedOn = DateTime.Now,
                    SubscriptionId = activeSubscription.SubId
                };

                _context.SubscriptionTransaction.Add(transaction);
                await _context.SaveChangesAsync();

                try
                {
                    string paymentLink = await _paymentService.CreateSubPaymentLink(
                        merchantTxnId,
                        totalAmount);

                    string? transId = null;

                    try
                    {
                        var uri = new Uri(paymentLink);
                        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        transId = queryParams["TransID"];
                    }
                    catch
                    {
                        transId = paymentLink.Contains("TransID=")
                            ? paymentLink.Split("TransID=").LastOrDefault()
                            : null;
                    }

                    transaction.PaymentLink = paymentLink;
                    transaction.PaymentRef = transId;
                    transaction.TransactionRefId = merchantTxnId;
                    transaction.Status = "PENDING";
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    transaction.PaymentLink = "";
                    transaction.TransactionRefId = "";
                    transaction.Status = "FAILED";
                    await _context.SaveChangesAsync();
                    await tx.RollbackAsync();
                    throw new Exception("MSWIPE PAYMENT ERROR : " + ex.Message);
                }

                foreach (var addon in addons)
                {
                    var addonPrice = addon.PlanDurations
                        .Where(d => d.IsActive)
                        .Select(d => d.Price)
                        .FirstOrDefault();

                    _context.CompanySubscriptionAddon.Add(new XRS_CompanySubscriptionAddon
                    {
                        CompanyId = dto.CompanyId,
                        MainPlanId = activeSubscription.SubId,
                        PlanId = addon.PlanId,
                        Amount = addonPrice,
                        UserCount = addon.PlanUsers,
                        Status = "PENDING",
                        CreatedOn = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new AddAddonResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    PaymentLink = transaction.PaymentLink,
                    PaymentStatus = transaction.Status,
                    Message = string.IsNullOrEmpty(transaction.PaymentLink)
                        ? "Addon request created. Payment gateway temporarily unavailable."
                        : "Payment link generated successfully"
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        public async Task<PaymentStatusResponseDto?> CheckPaymentStatusAsync(string transactionId)
        {
            var transaction = await _context.SubscriptionTransaction
                .FirstOrDefaultAsync(t => t.TransactionRefId == transactionId);

            if (transaction == null)
            {
                return null;
            }

            if (transaction.Status == "SUCCESS" ||
                transaction.Status == "FAILED" ||
                transaction.Status == "USERCANCELLED")
            {
                return new PaymentStatusResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    SubscriptionStatus = transaction.Status,
                    PaymentStatus = transaction.Status,
                    Message = $"Transaction already {transaction.Status}."
                };
            }

            var mswipeResponse = await _paymentService.CheckSubTransactionStatusAsync(transaction.PaymentRef);


            if (mswipeResponse?.Data == null || mswipeResponse.Data.Count == 0)
            {
                return new PaymentStatusResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    SubscriptionStatus = transaction.Status,
                    PaymentStatus = mswipeResponse?.ResponseMessage ?? "NOT_FOUND",
                    Message = "Transaction not yet initiated at gateway. Please try again shortly."
                };
            }


            var latestTxn = mswipeResponse.Data.First();

            if (latestTxn.Payment_Status == 1)
            {
                transaction.Status = "SUCCESS";
                transaction.ModifiedOn = DateTime.Now;

                if (transaction.SubscriptionId.HasValue)
                {
                    var subscription = await _context.CompanySubscriptions
                        .FirstOrDefaultAsync(s => s.SubId == transaction.SubscriptionId.Value);

                    if (subscription != null)
                    {
                        subscription.Status = "SUCCESS";
                    }

                    var addons = await _context.CompanySubscriptionAddon
                        .Where(a => a.MainPlanId == transaction.SubscriptionId.Value && a.Status == "PENDING")
                        .ToListAsync();

                    foreach (var addon in addons)
                    {
                        addon.Status = "SUCCESS";
                    }
                }

                await _context.SaveChangesAsync();

                return new PaymentStatusResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    SubscriptionStatus = "SUCCESS",
                    PaymentStatus = latestTxn.Payment_Desc ?? "SUCCESS",
                    Message = "Payment successful. Subscription activated."
                };
            }
            else if (latestTxn.Payment_Status == 0)
            {
                await ExpirePendingSubscriptions(transaction.CompanyId, transaction.SubscriptionId);

                transaction.Status = "FAILED";
                transaction.ModifiedOn = DateTime.Now;
                await _context.SaveChangesAsync();

                return new PaymentStatusResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    SubscriptionStatus = "FAILED",
                    PaymentStatus = latestTxn.Payment_Desc ?? "FAILED",
                    Message = "Payment failed."
                };
            }
            else
            {
                return new PaymentStatusResponseDto
                {
                    TransactionId = transaction.TransactionRefId,
                    SubscriptionStatus = transaction.Status,
                    PaymentStatus = latestTxn.Payment_Desc ?? "PENDING",
                    Message = "Payment is still pending. Please wait for confirmation."
                };
            }
        }
        public async Task<int> ExpireOutdatedSubscriptionsAsync()
        {
            var now = DateTime.Now;

            var expiredCandidates = await _context.CompanySubscriptions
                .Where(s =>
                    s.SubscriptionEndDate < now &&
                    (s.Status == "ACTIVE" || s.Status == "TRIAL"))
                .ToListAsync();

            if (!expiredCandidates.Any())
            {
                return 0;
            }

            int updatedCount = 0;


            var groupedByCompany = expiredCandidates.GroupBy(s => s.CompanyId);

            foreach (var companyGroup in groupedByCompany)
            {
                var companyId = companyGroup.Key;

                var latestSubscriptionForCompany = await _context.CompanySubscriptions
                    .Where(s => s.CompanyId == companyId)
                    .OrderByDescending(s => s.SubscriptionStartDate)
                    .FirstOrDefaultAsync();

                foreach (var subscription in companyGroup)
                {

                    if (latestSubscriptionForCompany == null ||
                        subscription.SubId != latestSubscriptionForCompany.SubId)
                    {
                        continue;
                    }

                    subscription.Status = subscription.PlanId == 0
                        ? "TRIAL_EXPIRED"
                        : "EXPIRED";

                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return updatedCount;
        }
    }

}
