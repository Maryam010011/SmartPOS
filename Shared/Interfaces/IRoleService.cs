using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Roles;

namespace SmartPOS.Shared.Interfaces;

public interface IRoleService
{
    Task<ApiResponse<List<RoleDto>>> GetAllRoles();
    Task<ApiResponse<RoleDetailDto>> GetRoleById(int id);
    Task<ApiResponse<RoleDto>> CreateRole(CreateRoleDto dto);
    Task<ApiResponse<RoleDto>> UpdateRole(int id, UpdateRoleDto dto);
    Task<ApiResponse> DeleteRole(int id);
    Task<ApiResponse<PermissionsDto>> GetRolePermissions(int id);
    Task<ApiResponse> UpdateRolePermissions(int id, PermissionsDto permissions);
}
