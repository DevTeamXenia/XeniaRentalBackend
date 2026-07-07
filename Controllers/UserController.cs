using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.Dictionnary;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.UserRole;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XeniaRentalBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

               
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<XRS_Users>>> GetUserByCompanyId(int companyId, int userId)

        {
            var branches = await _userRepository.GetUserByCompanyId(companyId, userId);
            if (branches == null || !branches.Any())
            {
                return NotFound(new { Status = "Error", Message = "No User found for the given Company ID." });
            }
            return Ok(new { Status = "Success", Data = branches });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<XRS_Users>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { Status = "Error", Message = "User not found." });
            }
            return Ok(new { Status = "Success", Data = user });
        }

               


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] DTOs.CreateUser user)
        {
            if (user == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid userRoll data." });
            }

            var createdUser = await _userRepository.CreateUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, new { Status = "Success", Data = createdUser });
        }


               

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserSetting(int id, [FromBody] XRS_Users user)
        {
            if (user == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid user data" });
            }

            var updated = await _userRepository.UpdateUserSetting(id, user);
            if (!updated)
            {
                return NotFound(new { Status = "Error", Message = "User not found or update failed." });
            }

            return Ok(new { Status = "Success", Message = "User updated successfully." });
        }



       

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSettings(int id)
        {
            var deleted = await _userRepository.DeleteUserSetting(id);
            if (!deleted)
            {
                return NotFound(new { Status = "Error", Message = "User not found or delete failed." });
            }

            return Ok(new { Status = "Success", Message = "User deleted successfully." });
        }


        [HttpGet("usertypes")]
        public IActionResult GetUserTypes()
        {
            return Ok(UserTypeProvider.UserTypes);
        }

        
    }
}
