using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace SmartPOS.Services.MaryamJ;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _cachedPrincipal = new(new ClaimsIdentity());
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cachedPrincipal.Identity?.IsAuthenticated == true)
        {
            return new AuthenticationState(_cachedPrincipal);
        }

        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                var identity = CreateIdentity(token);
                _cachedPrincipal = new ClaimsPrincipal(identity);
                return new AuthenticationState(_cachedPrincipal);
            }
        }
        catch
        {
        }

        return new AuthenticationState(Anonymous);
    }

    public void MarkUserAsAuthenticated(string token)
    {
        var identity = CreateIdentity(token);
        _cachedPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedPrincipal)));
    }

    public void MarkUserAsLoggedOut()
    {
        _cachedPrincipal = Anonymous;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }

    private static ClaimsIdentity CreateIdentity(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var claims = new List<Claim>();

        foreach (var claim in jwt.Claims)
        {
            var claimType = claim.Type switch
            {
                "sub" or ClaimTypes.NameIdentifier => ClaimTypes.NameIdentifier,
                "name" or ClaimTypes.Name => ClaimTypes.Name,
                "email" or ClaimTypes.Email => ClaimTypes.Email,
                "role" or ClaimTypes.Role => ClaimTypes.Role,
                _ => claim.Type
            };

            if (!claims.Any(c => c.Type == claimType))
            {
                claims.Add(new Claim(claimType, claim.Value));
            }
        }

        return new ClaimsIdentity(claims, "jwt");
    }
}
