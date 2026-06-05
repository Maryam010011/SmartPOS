using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.AuditLogs;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Policy = "AdminOnly")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetAll([FromQuery] AuditLogFilterDto filter)
        {
            var response = await _auditLogService.GetAuditLogs(filter);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AuditLogDto>>> GetById(int id)
        {
            var response = await _auditLogService.GetLogById(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetByUser(int userId)
        {
            var response = await _auditLogService.GetLogsForUser(userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("entity")]
        public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetByEntity(
            [FromQuery] string module, [FromQuery] int entityId)
        {
            var response = await _auditLogService.GetLogsForEntity(module, entityId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
