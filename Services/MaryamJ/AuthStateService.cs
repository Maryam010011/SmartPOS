using System;
using SmartPOS.Shared.DTOs.Auth;

namespace SmartPOS.Services.MaryamJ;

/// <summary>
/// Scoped service for holding client-side Blazor authentication state.
/// Persisted via ProtectedLocalStorage through CustomAuthStateProvider.
/// </summary>
public class AuthStateService
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int UserId { get; set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public event Action? OnChange;

    public void SetUser(LoginResponseDto response)
    {
        if (response?.User == null) return;

        Token = response.Token;
        UserId = response.User.Id;
        UserName = response.User.Name;
        UserRole = response.User.RoleName;
        Email = response.User.Email;

        NotifyStateChanged();
    }

    public void Logout()
    {
        Token = string.Empty;
        UserName = string.Empty;
        UserRole = string.Empty;
        Email = string.Empty;
        UserId = 0;

        NotifyStateChanged();
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}
