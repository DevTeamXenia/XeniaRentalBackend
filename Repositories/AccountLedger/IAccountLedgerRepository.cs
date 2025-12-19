using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Ledger
{
    public interface IAccountLedgerRepository
    {
        Task<IEnumerable<XRS_AccountLedger>> GetLedgers();

        Task<IEnumerable<XRS_AccountLedger>> GetLedgerDetails(int companyId, bool isgroup);

        Task<IEnumerable<XRS_AccountLedger>> GetLedgerbyId(int ledgerId);

        Task<Models.XRS_AccountLedger> CreateLedger(XRS_AccountLedger ledger);
   
        Task<bool> UpdateLedger(int id, XRS_AccountLedger ledger);
       
    }
}
