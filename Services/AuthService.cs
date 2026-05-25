using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Data;
using SmartPOS.Models;

namespace SmartPOS.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Verifies user credentials and logs the login action into their role-specific log table.
    /// </summary>
    public async Task<(string? Token, User? User)> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
                return (null, null);

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (null, null);

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            // Log successful login action based on role
            string roleName = user.Role?.Name ?? "Customer";
            await LogUserActionAsync(user.Id, roleName, "User logged in successfully.");

            return (token, user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection or login error: {ex.Message}");
            return (null, null);
        }
    }

    /// <summary>
    /// Inserts an action log into the unified AuditLog table.
    /// </summary>
    public async Task<bool> LogUserActionAsync(int userId, string role, string actionDetails)
    {
        try
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = actionDetails,
                Module = role,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Safe fallback logging for connection errors or schema issues
            Console.WriteLine($"Error writing action log to database: {ex.Message}");
            return false;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "FallbackSecretKeyForSmartPOSApplication";
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (user.Role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
