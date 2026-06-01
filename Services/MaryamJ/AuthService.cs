using System.Net.Http.Json;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Auth;

namespace SmartPOS.Services.MaryamJ;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<LoginResult?> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/auth/login", new { email, password });
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
            if (apiResponse?.Success != true || apiResponse.Data == null) return null;

            var data = apiResponse.Data;
            return new LoginResult
            {
                Token = data.Token,
                User = new UserInfo
                {
                    Id = data.User.Id,
                    Name = data.User.Name,
                    Email = data.User.Email,
                    Role = new RoleInfo { Name = data.User.RoleName }
                }
            };
        }
        catch
        {
            return null;
        }
    }
}
