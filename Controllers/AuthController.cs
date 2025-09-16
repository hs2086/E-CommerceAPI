using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_COMMERCEAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.RegisterAsync(request);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.LoginAsync(request);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost("addrole")]
        [Authorize]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.AddRoleAsync(request);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(request);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.RefreshTokenAsync(request);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
                return BadRequest("Invalid user");

            var result = await authService.LogoutAsync(userId);

            if (!result)
                return BadRequest("Logout failed");

            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await authService.ChangePasswordAsync(userId, request);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password changed successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.ForgotPasswordAsync(request.Email);

            return Ok(new { Message = result });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.ResetPasswordAsync(request);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await authService.VerifyEmailAsync(request);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("Email verified successfully.");
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.ResendVerificationEmailAsync(request);


            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("The Verification is sent");
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
                return BadRequest("Invalid user");
            var result = await authService.GetCurrentUserAsync(userId);
            if (result == null)
                return NotFound("User not found");
            return Ok(result);
        }

    }
}
