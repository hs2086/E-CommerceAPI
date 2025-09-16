using E_COMMERCEAPI.DTOs;
using Microsoft.AspNetCore.Identity;

namespace E_COMMERCEAPI.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDTO model);
        Task<AuthModel> LoginAsync(LoginDTO model);
        Task<string> AddRoleAsync(AddRoleDTO model);
        Task<AuthModel> RefreshTokenAsync(TokenRequestDTO model);
        Task<bool> LogoutAsync(string userId);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDTO model);
        Task<string> ForgotPasswordAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO model);
        Task<IdentityResult> VerifyEmailAsync(VerifyEmailDTO model);
        Task<IdentityResult> ResendVerificationEmailAsync(ResendVerificationDTO model);
        Task<ProfileDTO> GetCurrentUserAsync(string userId);
    }
}
