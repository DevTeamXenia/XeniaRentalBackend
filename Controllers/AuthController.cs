using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Repositories.Auth;
using XeniaRentalBackend.Service.Common;


namespace XeniaRentalBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelperService _jwtHelperService;

        public AuthController(IAuthRepository authRepository, JwtHelperService jwtHelperService)
        {
            _authRepository = authRepository;
            _jwtHelperService = jwtHelperService;
        }

 

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authRepository.AuthenticateAdminUser(request);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = "User does not exist."
                    });
                }

                var token = _authRepository.GenerateJwtAdminToken(user);

                return Ok(new
                {
                    Status = "Success",
                    Message = "Login successful.",
                    Token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "Error",
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }


        [HttpPost]
        [Route("OTP/login")]
        public async Task<IActionResult> GenerateLoginOTP([FromBody] LoginOTPDTO request)
        {
            return await _authRepository.GenerateLoginOTPAsync(request);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(string userName, string password, int companyId, string otp, string? deviceToken)
        {
            try
            {
                var user = await _authRepository.AuthenticateUser(userName, companyId, otp, deviceToken);

                if (user == null)
                {
                    return NotFound(new { Status = "Error", Message = "User does not exist." });
                }

                var token = _authRepository.GenerateJwtCustomerToken(user);
                return Ok(new { Status = "Success", Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = "Error", Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("OTP/forgotPassword")]
        public async Task<IActionResult> GenerateForgotPasswordOTP([FromBody] ForgetPasswordOTPDTO request)
        {
            return await _authRepository.GenerateForgotPasswordOTP(request);
        }


        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForegtPasswordDTO request)
        {
            try
            {
                var result = await _authRepository.ResetUserPassword(request);
                if (!result)
                {
                    return Unauthorized(new { Status = "Error", Message = "Password reset faild." });
                }

                return Ok(new { Status = "Success", Message = "Password has been reset successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = "Error", Message = ex.Message });
            }
        }

  


    }
}
