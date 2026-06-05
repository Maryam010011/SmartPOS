using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Auth;

namespace SmartPOS.Shared.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> Login(LoginRequestDto request);
    Task<ApiResponse<LoginResponseDto>> Register(RegisterDto request);
    Task<ApiResponse> ForgotPassword(string email);
    Task<ApiResponse> ResetPassword(string email, string code, string newPassword);
}
