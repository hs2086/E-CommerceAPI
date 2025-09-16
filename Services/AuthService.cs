﻿using E_COMMERCEAPI.DTOs;
using E_COMMERCEAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_COMMERCEAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailService emailService;
        private readonly JWT jWT;
        private readonly IConfiguration configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jWT, IEmailService emailService, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
            this.jWT = jWT.Value;
            this.configuration = configuration;
        }


        public async Task<AuthModel> RegisterAsync(RegisterDTO model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            var user = new ApplicationUser
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Country = model.Country,
                State = model.State,
                City = model.City,
                StreetAddress = model.StreetAddress,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }

            await userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);


            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var frontendUrl = configuration.GetValue<string>("DM");
            var link = $"{frontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailAsync(user.Email, $"Verify your email Click here: {link}");


            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                Email = user.Email,
                UserName = user.UserName,
                Roles = (await userManager.GetRolesAsync(user)).ToList(),
                Message = "User Registered Successfully, Go to your email to verify you email"
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims).Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWT.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jWT.Issuer,
                audience: jWT.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jWT.ExpiryInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<AuthModel> LoginAsync(LoginDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel
                {
                    IsAuthenticated = false,
                    Message = "Email or Password is incorrect!"
                };
            }
            if (!user.EmailConfirmed)
            {
                return new AuthModel
                {
                    IsAuthenticated = false,
                    Message = "Email is not confirmed!"
                };
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            if (user.RefreshToken != null && user.RefreshTokenExpiryTime > DateTime.UtcNow)
            {
                return new AuthModel
                {
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = (await userManager.GetRolesAsync(user)).ToList(),
                    Message = "Login Successful!"
                };
            }
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                Email = user.Email,
                UserName = user.UserName,
                Roles = (await userManager.GetRolesAsync(user)).ToList(),
                Message = "Login Successful!"
            };
        }
        public async Task<string> AddRoleAsync(AddRoleDTO model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is null || !await roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role.";

            if (await userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role.";

            var result = await userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "Failed to add role to user.";
        }
        public async Task<AuthModel> RefreshTokenAsync(TokenRequestDTO model)
        {
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == model.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new AuthModel { IsAuthenticated = false, Message = "Invalid refresh token" };

            var jwtToken = await CreateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                Email = user.Email,
                UserName = user.UserName,
                Roles = (await userManager.GetRolesAsync(user)).ToList(),
                Message = "Token Refreshed Successfully!"
            };
        }
        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDTO model)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }
        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return "Something is wrong.";

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var FrontendUrl = configuration.GetValue<string>("DM");
            var resetLink = $"{FrontendUrl}/reset-password?email={email}&token={Uri.EscapeDataString(token)}";
            await emailService.SendEmailAsync(user.Email, $"Reset Password Link\n Please reset your password by clicking this link: {resetLink}");
            return "Password reset link has been sent to your email.";

        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var decodedToken = Uri.UnescapeDataString(model.Token);

            return await userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
        }
        public async Task<IdentityResult> VerifyEmailAsync(VerifyEmailDTO model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Something is wrong." });

            var result = await userManager.ConfirmEmailAsync(user, model.Token);
            return result;
        }
        public async Task<IdentityResult> ResendVerificationEmailAsync(ResendVerificationDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Something is wrong." });
            if (user.EmailConfirmed) return IdentityResult.Failed(new IdentityError { Description = "Email is already confirmed." });

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var frontendUrl = configuration.GetValue<string>("DM");
            var link = $"{frontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailAsync(user.Email, $"Verify your email\n Please confirm your account by clicking this link: {link}");

            return IdentityResult.Success;
        }
        public async Task<ProfileDTO> GetCurrentUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return null;
            return new ProfileDTO
            {
                Email = user.Email,
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber
            };

        }

    }
}
