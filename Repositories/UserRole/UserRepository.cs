using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Numerics;
using XeniaRentalBackend.Models;
namespace XeniaRentalBackend.Repositories.UserRole
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<XRS_Users>> GetUsers()
        {
            
            return await _context.Users.Where(u => u.IsActive == true)
                .Select(u => new Models.XRS_Users
                {
                    UserId = u.UserId,
                     UserType= u.UserType,
                     UserName= u.UserName,
                     CompanyId= u.CompanyId,
                     Password= u.Password,
                     UsetTypeName= u.UserRole != null ? u.UserRole.UserRoleName : null,
                    IsActive= u.IsActive,
                    Email= u.Email,
                    Phone=u.Phone

                }).ToListAsync();


        }
        public async Task<IEnumerable<XRS_UserRole>> GetUserRoles()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<IEnumerable<Models.XRS_Users>> GetUserByCompanyId(int companyId)
        {

            return await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Select(u => new Models.XRS_Users
                {
                    UserId = u.UserId,
                    UserType = u.UserType,
                    UserName = u.UserName,
                    CompanyId = u.CompanyId,
                    Password = u.Password,
                    UsetTypeName = u.UserRole != null ? u.UserRole.UserRoleName : null,
                    IsActive = u.IsActive,
                    Email = u.Email,
                    Phone = u.Phone

                }).ToListAsync();

        }

        public async Task<IEnumerable<Models.XRS_Users>> GetUserById(int Id)
        {

            return await _context.Users
                .Where(u => u.UserId == Id)
                .Select(u => new Models.XRS_Users
                {
                    UserId = u.UserId,
                    UserType = u.UserType,
                    UserName = u.UserName,
                    CompanyId = u.CompanyId,
                    Password = u.Password,
                    UsetTypeName = u.UserRole != null ? u.UserRole.UserRoleName : null,
                    IsActive = u.IsActive,
                    Email = u.Email,
                    Phone = u.Phone

                }).ToListAsync();

        }

        public async Task<Models.XRS_Users> CreateUser(DTOs.CreateUser dtoUsers)
        {

            var users = new Models.XRS_Users
            {
              UserName = dtoUsers.UserName,
              Password = dtoUsers.Password,
              CompanyId=dtoUsers.CompanyId,
              IsActive=dtoUsers.IsActive,
              UserType = dtoUsers.UserType,
              CreatedDate = dtoUsers.CreatedDate,
              Modifieddate=dtoUsers.Modifieddate,
              Email = dtoUsers.Email,
              Phone = dtoUsers.Phone

            };
            await _context.Users.AddAsync(users);
            await _context.SaveChangesAsync();
            return users;

        }

        public async Task<bool> UpdateUserSetting(int id, XRS_Users updatedUser)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (existingUser == null) return false;

            existingUser.UserName = updatedUser.UserName ?? existingUser.UserName;
            existingUser.Password = updatedUser.Password ?? existingUser.Password;
            existingUser.IsActive = updatedUser.IsActive;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Email = updatedUser.Email;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserSetting(int id)
        {
            var accountgroupsettings = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (accountgroupsettings == null) return false;
            accountgroupsettings.IsActive = false;
            accountgroupsettings.Modifieddate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
