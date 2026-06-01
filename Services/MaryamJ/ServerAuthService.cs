using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Auth;
using SmartPOS.Shared.DTOs.Users;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services.MaryamJ;

public class ServerAuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    private static readonly Dictionary<string, string> ResetCodes = new();

    public ServerAuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<LoginResponseDto>.Fail("Invalid email or password");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());

            if (user == null)
                return ApiResponse<LoginResponseDto>.Fail("Invalid email or password");

            if (!user.IsActive)
                return ApiResponse<LoginResponseDto>.Fail("Account is deactivated");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ApiResponse<LoginResponseDto>.Fail("Invalid email or password");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
            var roleName = role?.RoleName ?? "Customer";
            var token = GenerateJwtToken(user.Id, user.Name, user.Email, roleName);

            var expiryHours = GetExpiryHours();
            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(expiryHours),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    RoleName = roleName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                }
            }, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.Fail($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LoginResponseDto>> Register(RegisterDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ApiResponse<LoginResponseDto>.Fail("Email and password are required.");

            if (dto.Password != dto.ConfirmPassword)
                return ApiResponse<LoginResponseDto>.Fail("Passwords do not match.");

            if (dto.Password.Length < 6)
                return ApiResponse<LoginResponseDto>.Fail("Password must be at least 6 characters.");

            var emailNormalized = dto.Email.Trim().ToLower();
            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == emailNormalized);
            if (exists)
                return ApiResponse<LoginResponseDto>.Fail("Email is already registered.");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email.Trim(),
                PasswordHash = hash,
                RoleId = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var userInDb = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == newUser.Id);

            var roleName = userInDb?.Role?.RoleName ?? "Customer";
            var token = GenerateJwtToken(newUser.Id, newUser.Name, newUser.Email, roleName);

            var expiryHours = GetExpiryHours();
            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(expiryHours),
                User = new UserDto
                {
                    Id = newUser.Id,
                    Name = newUser.Name,
                    Email = newUser.Email,
                    RoleId = newUser.RoleId,
                    RoleName = roleName,
                    IsActive = newUser.IsActive,
                    CreatedAt = newUser.CreatedAt
                }
            }, "Registration successful.");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.Fail($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ForgotPassword(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return ApiResponse.Fail("Email is required.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());
            if (user == null)
                return ApiResponse.Fail("Email not found");

            var code = Random.Shared.Next(100000, 999999).ToString();
            ResetCodes[email.Trim().ToLower()] = code;

            return ApiResponse.Ok(code);
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ResetPassword(string email, string code, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(newPassword))
                return ApiResponse.Fail("Email, code, and new password are required.");

            var emailKey = email.Trim().ToLower();
            if (!ResetCodes.TryGetValue(emailKey, out var storedCode) || storedCode != code.Trim())
                return ApiResponse.Fail("Invalid or expired reset code");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == emailKey);
            if (user == null)
                return ApiResponse.Fail("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            ResetCodes.Remove(emailKey);

            return ApiResponse.Ok("Password reset successful");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed to reset password: {ex.Message}");
        }
    }

    public async Task<ApiResponse> Logout(string token)
    {
        await Task.CompletedTask;
        return ApiResponse.Ok("Logged out successfully.");
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshToken(string refreshToken)
    {
        await Task.CompletedTask;
        return ApiResponse<LoginResponseDto>.Fail("Token refresh not implemented.");
    }

    private string GenerateJwtToken(int userId, string name, string email, string roleName)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SmartPOSSecretKey2026AirUniversityCS284L");
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, roleName)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SmartPOS",
            audience: _configuration["Jwt:Audience"] ?? "SmartPOSUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(GetExpiryHours()),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private double GetExpiryHours()
    {
        return double.TryParse(_configuration["Jwt:ExpiryHours"], out var hours) ? hours : 8;
    }
}