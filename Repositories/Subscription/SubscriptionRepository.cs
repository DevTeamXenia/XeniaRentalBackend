using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.Dtos;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Service.Payment;

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


        public async Task<List<PlanWithModulesDto>> GetMainPlansAsync()
        {
            var plans = await _context.SubscribePlan
                .Where(p => p.PlanActive && !p.PlanIsAddOn)
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

                result.Add(new PlanWithModulesDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    PlanDescription = plan.PlanDescription,
                    PlanPrice = plan.PlanPrice,
                    PlanDurationDays = plan.PlanDurationDays,
                    Modules = modules
                });
            }

            return result;
        }



        public async Task<RenewSubscriptionResponseDto?> RenewSubscriptionAsync(RenewSubscriptionDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var mainPlan = await _context.SubscribePlan
                .FirstOrDefaultAsync(p => p.PlanId == dto.PlanId && p.PlanActive && !p.PlanIsAddOn);

            if (mainPlan == null) return null;

            decimal totalAmount = mainPlan.PlanPrice;
            var merchantTxnId = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N")[..8]}";
         
            var transaction = new XRS_SubscriptionTransaction
            {
                CompanyId = dto.CompanyId,
                Amount = totalAmount,
                PaymentProvider = "MSWIPE",
                TransactionRefId = merchantTxnId,
                Status = "INITIATED",
                CreatedOn = DateTime.Now
            };

            _context.SubscriptionTransaction.Add(transaction);
            await _context.SaveChangesAsync();

            var paymentLink = await _paymentService.CreatePaymentLink(
                transaction.TransactionRefId,
                totalAmount);

            transaction.PaymentLink = paymentLink;
            await _context.SaveChangesAsync();


            var startDate = DateTime.Now;
            var endDate = startDate.AddDays(mainPlan.PlanDurationDays);

            var subscription = new XRS_CompanySubscription
            {
                CompanyId = dto.CompanyId,
                PlanId = mainPlan.PlanId,
                SubscriptionDate = DateTime.Now,
                SubscriptionStartDate = startDate,
                SubscriptionEndDate = endDate,
                SubscriptionAmount = mainPlan.PlanPrice,
                SubscriptionDays = mainPlan.PlanDurationDays,
                Status = "PENDING"
            };

            _context.CompanySubscription.Add(subscription);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new RenewSubscriptionResponseDto
            {
                TransactionId = transaction.TransactionRefId,
                PaymentLink = paymentLink
            };
        }


        public async Task<MswipeTransactionStatusResponse> CheckTransactionStatusAsync(string transId)
        {
            var statusRequest = new
            {
                id = transId,
            };

            var statusResponse = await _httpClient.PostAsJsonAsync(
                "https://dcuat.mswipetech.co.in/ipg/api/getPBLTransactionDetails", statusRequest);

            var rawJson = await statusResponse.Content.ReadAsStringAsync();

            if (!statusResponse.IsSuccessStatusCode)
                throw new Exception($"Failed to check transaction status. Raw Response: {rawJson}");

            var result = System.Text.Json.JsonSerializer.Deserialize<MswipeTransactionStatusResponse>(
                rawJson,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null || !string.Equals(result.Status, "True", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Transaction status check failed: " + result?.ResponseMessage);

            return result;
        }


        public async Task<bool> UpdatePaymentStatusAsync(string transactionRefId, bool isSuccess)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var transaction = await _context.SubscriptionTransaction
                .FirstOrDefaultAsync(x => x.TransactionRefId == transactionRefId);

            if (transaction == null) return false;

            transaction.TransactionRefId = transactionRefId;
            transaction.Status = isSuccess ? "SUCCESS" : "FAILED";

            if (!isSuccess)
            {
                await ExpirePendingSubscriptions(transaction.CompanyId);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }

            var previousSubs = await _context.CompanySubscription
                .Where(x =>
                    x.CompanyId == transaction.CompanyId &&
                    (x.Status == "ACTIVE" || x.Status == "TRIAL"))
                .ToListAsync();

            foreach (var sub in previousSubs)
            {
                if (sub.Status == "TRIAL")
                    sub.Status = "TRIAL_EXPIRED";
                else
                    sub.Status = "EXPIRED";
            }

            var subscriptions = await _context.CompanySubscription
                .Where(x => x.CompanyId == transaction.CompanyId && x.Status == "PENDING")
                .ToListAsync();

            foreach (var s in subscriptions)
                s.Status = "ACTIVE";


            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return true;
        }



        private async Task ExpirePendingSubscriptions(int companyId)
        {
            var pendingSubs = await _context.CompanySubscription
                .Where(x => x.CompanyId == companyId && x.Status == "PENDING")
                .ToListAsync();

            foreach (var s in pendingSubs)
                s.Status = "EXPIRED";
     

            await _context.SaveChangesAsync();
        }
    }

}
