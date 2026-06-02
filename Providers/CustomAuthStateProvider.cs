using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace SmartPOS.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private static readonly AuthenticationState _anonymous =
        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // During SSR pre-rendering, JS interop is unavailable — return anonymous gracefully
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            // JS interop not yet available (pre-rendering phase) — safe fallback
            return _anonymous;
        }
    }

    public void MarkUserAsAuthenticated(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null) return claims;

        // Handle Role claim specially (can be array or single value)
        if (keyValuePairs.TryGetValue("role", out var roles) && roles != null)
        {
            var rolesStr = roles.ToString()!.Trim();
            if (rolesStr.StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesStr);
                if (parsedRoles != null)
                    foreach (var r in parsedRoles)
                        claims.Add(new Claim(ClaimTypes.Role, r));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, rolesStr));
            }
            keyValuePairs.Remove("role");
        }

        claims.AddRange(keyValuePairs.Select(kvp =>
            new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty)));

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
