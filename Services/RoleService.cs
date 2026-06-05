using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.AuditLogs;
using SmartPOS.Shared.DTOs.Roles;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Data;
using SmartPOS.Models;

namespace SmartPOS.Services.MaryamJ
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuditLogService _auditLog;

        public RoleService(AppDbContext dbContext, IAuditLogService auditLog)
        {
            _dbContext = dbContext;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<List<RoleDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _dbContext.Roles
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        RoleName = r.RoleName,
                        UserCount = r.Users != null ? r.Users.Count : 0
                    })
                    .ToListAsync();

                return ApiResponse<List<RoleDto>>.Ok(roles);
            }
            catch (Exception)
            {
                // Fallback: return empty list if DB unavailable
                return ApiResponse<List<RoleDto>>.Ok(new List<RoleDto>());
            }
        }

        public async Task<ApiResponse<RoleDetailDto>> GetRoleById(int id)
        {
            var role = await _dbContext.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return ApiResponse<RoleDetailDto>.Fail($"Role with Id {id} not found.");

            var detail = new RoleDetailDto
            {
                Id = role.Id,
                RoleName = role.RoleName,
                UserCount = role.Users != null ? role.Users.Count : 0,
                Permissions = DeserializePermissions(role.Permissions)
            };

            return ApiResponse<RoleDetailDto>.Ok(detail);
        }

        public async Task<ApiResponse<RoleDto>> CreateRole(CreateRoleDto dto)
        {
            // Unique role name check
            var exists = await _dbContext.Roles
                .AnyAsync(r => r.RoleName == dto.RoleName);
            if (exists)
                return ApiResponse<RoleDto>.Fail($"Role name '{dto.RoleName}' already exists.");

            var role = new Role
            {
                RoleName = dto.RoleName,
                Permissions = JsonSerializer.Serialize(dto.Permissions)
            };

            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            await _auditLog.LogAction(new CreateAuditLogDto
            {
                UserId = 0,
                Action = "Created",
                Module = "RoleManagement",
                EntityId = role.Id,
                NewValues = JsonSerializer.Serialize(new { role.RoleName, Permissions = role.Permissions })
            });

            var result = new RoleDto { Id = role.Id, RoleName = role.RoleName, UserCount = 0 };
            return ApiResponse<RoleDto>.Ok(result, "Role created successfully.");
        }

        public async Task<ApiResponse<RoleDto>> UpdateRole(int id, UpdateRoleDto dto)
        {
            var role = await _dbContext.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return ApiResponse<RoleDto>.Fail($"Role with Id {id} not found.");

            // If role name changed, ensure uniqueness
            if (role.RoleName != dto.RoleName)
            {
                var exists = await _dbContext.Roles
                    .AnyAsync(r => r.RoleName == dto.RoleName && r.Id != id);
                if (exists)
                    return ApiResponse<RoleDto>.Fail($"Role name '{dto.RoleName}' already exists.");
            }

            var oldRoleName = role.RoleName;
            var oldPermissions = role.Permissions;

            role.RoleName = dto.RoleName;
            role.Permissions = JsonSerializer.Serialize(dto.Permissions);
            await _dbContext.SaveChangesAsync();

            await _auditLog.LogAction(new CreateAuditLogDto
            {
                UserId = 0,
                Action = "Updated",
                Module = "RoleManagement",
                EntityId = role.Id,
                OldValues = JsonSerializer.Serialize(new { RoleName = oldRoleName, Permissions = oldPermissions }),
                NewValues = JsonSerializer.Serialize(new { role.RoleName, Permissions = role.Permissions })
            });

            var result = new RoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName,
                UserCount = role.Users?.Count ?? 0
            };
            return ApiResponse<RoleDto>.Ok(result, "Role updated successfully.");
        }

        public async Task<ApiResponse> DeleteRole(int id)
        {
            var role = await _dbContext.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return ApiResponse.Fail($"Role with Id {id} not found.");

            // Block deletion if ANY user (active or inactive) is assigned
            if (role.Users != null && role.Users.Any())
                return ApiResponse.Fail(
                    $"Cannot delete role '{role.RoleName}' because {role.Users.Count} user(s) are assigned to it. " +
                    "Reassign all users before deleting this role.");

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();

            await _auditLog.LogAction(new CreateAuditLogDto
            {
                UserId = 0,
                Action = "Deleted",
                Module = "RoleManagement",
                EntityId = role.Id,
                OldValues = JsonSerializer.Serialize(new { role.RoleName, Permissions = role.Permissions })
            });

            return ApiResponse.Ok("Role deleted successfully.");
        }

        public async Task<ApiResponse<PermissionsDto>> GetRolePermissions(int id)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return ApiResponse<PermissionsDto>.Fail($"Role with Id {id} not found.");

            var perms = DeserializePermissions(role.Permissions);
            return ApiResponse<PermissionsDto>.Ok(perms);
        }

        public async Task<ApiResponse> UpdateRolePermissions(int id, PermissionsDto permissions)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return ApiResponse.Fail($"Role with Id {id} not found.");

            var oldPermissions = role.Permissions;

            role.Permissions = JsonSerializer.Serialize(permissions);
            await _dbContext.SaveChangesAsync();

            await _auditLog.LogAction(new CreateAuditLogDto
            {
                UserId = 0,
                Action = "Updated",
                Module = "RoleManagement",
                EntityId = role.Id,
                OldValues = JsonSerializer.Serialize(new { Permissions = oldPermissions }),
                NewValues = JsonSerializer.Serialize(new { Permissions = role.Permissions })
            });

            return ApiResponse.Ok("Permissions updated successfully.");
        }

        // â”€â”€ Helper â”€â”€
        private static PermissionsDto DeserializePermissions(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<PermissionsDto>(json) ?? new PermissionsDto();
            }
            catch
            {
                return new PermissionsDto();
            }
        }
    }
}
