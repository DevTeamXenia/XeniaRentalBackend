using Microsoft.EntityFrameworkCore;
using XeniaRentalBackend.DTOs;
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

        public async Task<IEnumerable<XRS_Users>> GetUserByCompanyId(int companyId)
        {
            return await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Select(u => new XRS_Users
                {
                    UserId = u.UserId,
                    UserType = u.UserType,
                    UserName = u.UserName,
                    CompanyId = u.CompanyId,
                    Password = u.Password,

                    UsetTypeName = u.UserRole != null
                                    ? u.UserRole.UserRoleName
                                    : null,

                    IsActive = u.IsActive,
                    Email = u.Email,
                    Phone = u.Phone,

                    UserMappings = u.UserMappings.Select(m => new XRS_UserMapping
                    {
                        UnitMapID = m.UnitMapID,
                        UserID = m.UserID,
                        PropID = m.PropID,
                        IsActive = m.IsActive,

                        PropertyName = m.Property != null
                                        ? m.Property.propertyName
                                        : null

                    }).ToList()

                }).ToListAsync();
        }

        public async Task<IEnumerable<XRS_Users>> GetUserById(int Id)
        {
            return await _context.Users
                .Where(u => u.UserId == Id)
                .Select(u => new XRS_Users
                {
                    UserId = u.UserId,
                    UserType = u.UserType,
                    UserName = u.UserName,
                    CompanyId = u.CompanyId,
                    Password = u.Password,

                    UsetTypeName = u.UserRole != null
                                    ? u.UserRole.UserRoleName
                                    : null,

                    IsActive = u.IsActive,
                    Email = u.Email,
                    Phone = u.Phone,

                    UserMappings = u.UserMappings.Select(m => new XRS_UserMapping
                    {
                        UnitMapID = m.UnitMapID,
                        UserID = m.UserID,
                        PropID = m.PropID,
                        IsActive = m.IsActive,

                        PropertyName = m.Property != null
                                        ? m.Property.propertyName
                                        : null

                    }).ToList()

                }).ToListAsync();
        }

        public async Task<XRS_Users> CreateUser(CreateUser dtoUsers)
        {
            var users = new XRS_Users
            {
                UserName = dtoUsers.UserName,
                Password = dtoUsers.Password,
                CompanyId = dtoUsers.CompanyId,
                IsActive = dtoUsers.IsActive,
                UserType = dtoUsers.UserType,
                CreatedDate = dtoUsers.CreatedDate,
                Modifieddate = dtoUsers.Modifieddate,
                Email = dtoUsers.Email,
                Phone = dtoUsers.Phone
            };

            await _context.Users.AddAsync(users);
            await _context.SaveChangesAsync();

            if (dtoUsers.UserMappings != null && dtoUsers.UserMappings.Any())
            {
                var mappings = dtoUsers.UserMappings.Select(x => new Models.XRS_UserMapping
                {
                    UserID = users.UserId,
                    PropID = x.PropID, 
                    IsActive = x.IsActive
                }).ToList();

                await _context.UserMapping.AddRangeAsync(mappings);
                await _context.SaveChangesAsync();
            }

            return users;
        }

        public async Task<bool> UpdateUserSetting(int id, XRS_Users updatedUser)
        {
            var existingUser = await _context.Users
                .Include(u => u.UserMappings)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (existingUser == null)
                return false;
            existingUser.UserName = updatedUser.UserName ?? existingUser.UserName;
            existingUser.Password = updatedUser.Password ?? existingUser.Password;
            existingUser.IsActive = updatedUser.IsActive;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Email = updatedUser.Email;
            existingUser.UserType = updatedUser.UserType;
            existingUser.CompanyId = updatedUser.CompanyId;
            existingUser.Modifieddate = DateTime.Now;


            if (existingUser.UserMappings != null &&
                existingUser.UserMappings.Any())
            {
                _context.UserMapping.RemoveRange(existingUser.UserMappings);
            }

            if (updatedUser.UserMappings != null &&
                updatedUser.UserMappings.Any())
            {
                var mappings = updatedUser.UserMappings.Select(m => new XRS_UserMapping
                {
                    UserID = existingUser.UserId,
                    PropID = m.PropID,
                    IsActive = m.IsActive
                }).ToList();

                await _context.UserMapping.AddRangeAsync(mappings);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserSetting(int id)
        {
            var accountgroupsettings = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (accountgroupsettings == null) return false;
            accountgroupsettings.IsActive = false;
            accountgroupsettings.Modifieddate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
