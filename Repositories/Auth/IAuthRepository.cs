
using Microsoft.AspNetCore.Mvc;
using XeniaRentalBackend.DTOs;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<XRS_Users?> AuthenticateAdminUser(LoginRequest request);
        
        Task<IActionResult> GenerateLoginOTPAsync(LoginOTPDTO request);
        Task<XRS_Tenant?> AuthenticateUser(string username, int companyId, string otp, string? deviceToken);

        Task<IActionResult> GenerateForgotPasswordOTP(ForgetPasswordOTPDTO request);

         Task<bool> ResetUserPassword(ForegtPasswordDTO request);

        string GenerateJwtAdminToken(XRS_Users user);

        string GenerateJwtCustomerToken(XRS_Tenant user);

    }
}
