using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.UserRole
{
    public interface IUserRepository
    { 
        Task<IEnumerable<Models.XRS_Users>> GetUserByCompanyId(int companyId, int userId);
        Task<IEnumerable<Models.XRS_Users>> GetUserById(int Id);
        Task<Models.XRS_Users> CreateUser(DTOs.CreateUser userSettings);
        Task<bool> UpdateUserSetting(int id, XRS_Users userSettings);
        Task<bool> DeleteUserSetting(int id);

    }
}
