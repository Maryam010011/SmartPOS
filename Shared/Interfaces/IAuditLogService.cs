using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.AuditLogs;

namespace SmartPOS.Shared.Interfaces;

public interface IAuditLogService
{
    Task LogAction(CreateAuditLogDto dto);
    Task<ApiResponse<List<AuditLogDto>>> GetAuditLogs(AuditLogFilterDto filter);
    Task<ApiResponse<AuditLogDto>> GetLogById(int id);
    Task<ApiResponse<List<AuditLogDto>>> GetLogsForUser(int userId);
    Task<ApiResponse<List<AuditLogDto>>> GetLogsForEntity(string module, int entityId);
}
