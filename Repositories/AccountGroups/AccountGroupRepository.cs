using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;


namespace XeniaRentalBackend.Repositories.AccountGroups
{
    public class AccountGroupRepository: IAccountGroupRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountGroupRepository(ApplicationDbContext context) 
        {
            _context = context;

        }

        public async Task<IEnumerable<XRS_AccountGroup>> GetAccountGroups()
        {

            return await _context.AccountGroups.ToListAsync();

        }

        public async Task<IEnumerable<XRS_AccountGroup>> GetAccountGroupsByCompanyId(int companyId)
        {

            return await _context.AccountGroups
                .Where(u => u.companyID == companyId)
                 .ToListAsync();

        }

        public async Task<IEnumerable<XRS_AccountGroup>> GetAccountGroupById(int accountGroupId)
        {

            return await _context.AccountGroups
                .Where(u => u.groupID == accountGroupId)
                 .ToListAsync();

        }

        public async Task<XRS_AccountGroup> CreateAccountGroups(XRS_AccountGroup accountgroups)
        {

            await _context.AccountGroups.AddAsync(accountgroups);
            await _context.SaveChangesAsync();
            return accountgroups;

        }

        public async Task<bool> DeleteAccountGroup(int id)
        {
            var accountgroupsettings = await _context.AccountGroups.FirstOrDefaultAsync(u => u.groupID == id);
            if (accountgroupsettings == null) return false;
            accountgroupsettings.isActive = false;
            accountgroupsettings.modifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAccountGroup(int id, XRS_AccountGroup updatedUser)
        {
            var updatedAccountGroup = await _context.AccountGroups.FirstOrDefaultAsync(u => u.groupID == id);
            if (updatedAccountGroup == null) return false;

            updatedAccountGroup.groupName = updatedAccountGroup.groupName ?? updatedAccountGroup.groupName;
            updatedAccountGroup.companyID = updatedAccountGroup.companyID;
            updatedAccountGroup.groupCode = updatedAccountGroup.groupCode;
            updatedAccountGroup.isActive = updatedAccountGroup.isActive;
            updatedAccountGroup.modifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
