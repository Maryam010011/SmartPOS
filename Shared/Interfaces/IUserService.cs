using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Users;

namespace SmartPOS.Shared.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<UserDto>>> GetAllUsers(UserFilterDto filter);
    Task<ApiResponse<UserDetailDto>> GetUserById(int id);
    Task<ApiResponse<UserDto>> CreateUser(CreateUserDto dto);
    Task<ApiResponse<UserDto>> UpdateUser(int id, UpdateUserDto dto);
    Task<ApiResponse> DeleteUser(int id);
    Task<ApiResponse> ActivateUser(int id);
    Task<ApiResponse> DeactivateUser(int id);
    Task<ApiResponse> BulkActivate(List<int> ids);
    Task<ApiResponse> BulkDeactivate(List<int> ids);
}
