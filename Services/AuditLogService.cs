using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.AuditLogs;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services.MaryamJ
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAction(CreateAuditLogDto dto)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = dto.UserId,
                    Action = dto.Action,
                    Module = dto.Module,
                    EntityId = dto.EntityId,
                    OldValues = dto.OldValues,
                    NewValues = dto.NewValues,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = dto.IPAddress
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Silently fail — audit logging should never break the calling operation
            }
        }

        public async Task<ApiResponse<List<AuditLogDto>>> GetAuditLogs(AuditLogFilterDto filter)
        {
            try
            {
                var query = _context.AuditLogs
                    .Include(al => al.User)
                    .AsQueryable();

                if (filter.DateFrom.HasValue)
                    query = query.Where(al => al.Timestamp >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                    query = query.Where(al => al.Timestamp <= filter.DateTo.Value);

                if (!string.IsNullOrWhiteSpace(filter.Module))
                    query = query.Where(al => al.Module == filter.Module);

                if (filter.UserId.HasValue)
                    query = query.Where(al => al.UserId == filter.UserId.Value);

                if (!string.IsNullOrWhiteSpace(filter.Action))
                    query = query.Where(al => al.Action == filter.Action);

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var term = filter.SearchTerm.Trim().ToLower();
                    query = query.Where(al =>
                        al.User.Name.ToLower().Contains(term) ||
                        al.User.Email.ToLower().Contains(term) ||
                        al.Action.ToLower().Contains(term) ||
                        al.Module.ToLower().Contains(term));
                }

                query = query.OrderByDescending(al => al.Timestamp);

                int page = filter.Page > 0 ? filter.Page : 1;
                int pageSize = filter.PageSize > 0 ? filter.PageSize : 20;

                var logs = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = logs.Select(MapToDto).ToList();
                return ApiResponse<List<AuditLogDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AuditLogDto>>.Fail("Failed to retrieve audit logs.", new() { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AuditLogDto>>> GetLogsForUser(int userId)
        {
            try
            {
                var logs = await _context.AuditLogs
                    .Include(al => al.User)
                    .Where(al => al.UserId == userId)
                    .OrderByDescending(al => al.Timestamp)
                    .ToListAsync();

                var dtos = logs.Select(MapToDto).ToList();
                return ApiResponse<List<AuditLogDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AuditLogDto>>.Fail("Failed to retrieve logs for user.", new() { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AuditLogDto>>> GetLogsForEntity(string module, int entityId)
        {
            try
            {
                var logs = await _context.AuditLogs
                    .Include(al => al.User)
                    .Where(al => al.Module == module && al.EntityId == entityId)
                    .OrderByDescending(al => al.Timestamp)
                    .ToListAsync();

                var dtos = logs.Select(MapToDto).ToList();
                return ApiResponse<List<AuditLogDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AuditLogDto>>.Fail("Failed to retrieve logs for entity.", new() { ex.Message });
            }
        }

        public async Task<ApiResponse<AuditLogDto>> GetLogById(int id)
        {
            try
            {
                var log = await _context.AuditLogs
                    .Include(al => al.User)
                    .FirstOrDefaultAsync(al => al.Id == id);

                if (log == null)
                    return ApiResponse<AuditLogDto>.Fail("Audit log not found.");

                return ApiResponse<AuditLogDto>.Ok(MapToDto(log));
            }
            catch (Exception ex)
            {
                return ApiResponse<AuditLogDto>.Fail("Failed to retrieve audit log.", new() { ex.Message });
            }
        }

        private static AuditLogDto MapToDto(AuditLog log)
        {
            return new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.User?.Name ?? "Unknown",
                UserEmail = log.User?.Email ?? "Unknown",
                Action = log.Action,
                Module = log.Module,
                EntityId = log.EntityId,
                OldValues = log.OldValues,
                NewValues = log.NewValues,
                Timestamp = log.Timestamp,
                IPAddress = log.IPAddress
            };
        }
    }
}
