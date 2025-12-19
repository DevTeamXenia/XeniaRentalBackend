
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.AccountGroups
{
    public interface IAccountGroupRepository
    {
        Task<IEnumerable<XRS_AccountGroup>> GetAccountGroups();
        Task<IEnumerable<XRS_AccountGroup>> GetAccountGroupsByCompanyId(int companyId);

        Task<IEnumerable<XRS_AccountGroup>> GetAccountGroupById(int accountGroupId);

        Task<XRS_AccountGroup> CreateAccountGroups(XRS_AccountGroup account);

        Task<bool> DeleteAccountGroup(int id);

        Task<bool> UpdateAccountGroup(int id, XRS_AccountGroup updatedUser);
 
    }
}
