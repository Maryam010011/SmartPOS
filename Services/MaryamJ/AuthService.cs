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

namespace SmartPOS.Services.MaryamJ;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<LoginResponseDto>.Fail("Email and password are required.");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());

            if (user == null)
                return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

            if (!user.IsActive)
                return ApiResponse<LoginResponseDto>.Fail("Account is deactivated. Contact admin.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

            var roleName = user.Role?.RoleName ?? "Customer";
            var token = GenerateJwtToken(user.Id, user.Name, user.Email, roleName);

            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
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
            }, "Login successful.");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.Fail($"Login failed: {ex.Message}");
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

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = GetSecretKey();
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateJwtToken(int userId, string name, string email, string roleName)
    {
        var key = GetSecretKey();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, roleName),
            new("role", roleName)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetExpiryMinutes()),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private byte[] GetSecretKey() =>
        Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

    private int GetExpiryMinutes() =>
        int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var min) ? min : 120;
}
