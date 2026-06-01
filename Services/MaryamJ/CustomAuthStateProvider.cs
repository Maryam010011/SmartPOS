using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartPOS.Shared.DTOs.Auth;

namespace SmartPOS.Services.MaryamJ;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _storage;
    private readonly AuthStateService _authState;
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ProtectedLocalStorage storage, AuthStateService authState)
    {
        _storage = storage;
        _authState = authState;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _storage.GetAsync<StoredAuthData>("authData");
            if (result.Success && result.Value is { Token: not null } data && !string.IsNullOrEmpty(data.Token))
            {
                _authState.Token = data.Token;
                _authState.UserId = data.UserId;
                _authState.UserName = data.UserName;
                _authState.UserRole = data.UserRole;
                _authState.Email = data.Email;
                _authState.NotifyStateChanged();

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()),
                    new Claim(ClaimTypes.Name, data.UserName),
                    new Claim(ClaimTypes.Email, data.Email),
                    new Claim(ClaimTypes.Role, data.UserRole)
                }, "jwt");

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
            // Storage not available during prerendering
        }

        return new AuthenticationState(Anonymous);
    }

    public async Task NotifyLoginAsync(LoginResponseDto response)
    {
        var data = new StoredAuthData
        {
            Token = response.Token,
            UserId = response.User.Id,
            UserName = response.User.Name,
            Email = response.User.Email,
            UserRole = response.User.RoleName
        };

        await _storage.SetAsync("authData", data);

        _authState.SetUser(response);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, data.UserId.ToString()),
            new Claim(ClaimTypes.Name, data.UserName),
            new Claim(ClaimTypes.Email, data.Email),
            new Claim(ClaimTypes.Role, data.UserRole)
        }, "jwt");

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task NotifyLogoutAsync()
    {
        try { await _storage.DeleteAsync("authData"); } catch { }
        _authState.Logout();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }

    private class StoredAuthData
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}
