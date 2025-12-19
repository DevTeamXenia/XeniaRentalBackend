using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Ledger
{
    public class AccountLedgerRepository:IAccountLedgerRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountLedgerRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_AccountLedger>> GetLedgers()
        {

            return await _context.Ledgers.ToListAsync();

        }


        public async Task<IEnumerable<XRS_AccountLedger>> GetLedgerDetails(int companyId, bool isgroup)
        {
            var query = _context.Ledgers
                .AsNoTracking()
                .Where(u => u.companyID == companyId);

            if (isgroup)
            {
                var indirectGroup = await _context.AccountGroups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.groupCode == "INDIRECT EXPENSES" && g.companyID == companyId);

                if (indirectGroup != null)
                {
                    int indirectExpensesGroupId = indirectGroup.groupID;
                    query = query.Where(l => l.groupID == indirectExpensesGroupId);
                }
                else
                {
                    return new List<XRS_AccountLedger>();
                }
            }

            return await query.ToListAsync();
        }


        public async Task<IEnumerable<XRS_AccountLedger>> GetLedgerbyId(int ledgerId)
        {

            return await _context.Ledgers
                .Where(u => u.ledgerID == ledgerId)
                 .ToListAsync();

        }

        public async Task<XRS_AccountLedger> CreateLedger(XRS_AccountLedger ledger)
        {

            await _context.Ledgers.AddAsync(ledger);
            await _context.SaveChangesAsync();
            return ledger;

        }

        public async Task<bool> UpdateLedger(int id, XRS_AccountLedger ledger)
        {
            var UpdatedLedger = await _context.Ledgers.FirstOrDefaultAsync(u => u.ledgerID == id);
            if (UpdatedLedger == null) return false;

            UpdatedLedger.ledgerName = ledger.ledgerName ?? ledger.ledgerName;
            UpdatedLedger.companyID = ledger.companyID;
            UpdatedLedger.ledgerCode = ledger.ledgerCode;
            UpdatedLedger.groupID = ledger.groupID;
            UpdatedLedger.isActive = ledger.isActive;
            await _context.SaveChangesAsync();
            return true;
        }

      
    }
}
