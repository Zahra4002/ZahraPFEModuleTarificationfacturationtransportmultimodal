// API/Controllers/AuditLogsController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogsController(IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Search audit logs with filters
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<List<AuditLogDto>>> SearchLogs([FromBody] AuditLogFilterDto filter)
        {
            try
            {
                var logs = await _auditLogService.SearchLogsAsync(filter);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get logs for reporting dashboard
        /// </summary>
        [HttpPost("reporting")]
        public async Task<ActionResult<List<AuditLogDto>>> GetReportingLogs([FromBody] AuditLogFilterDto filter)
        {
            try
            {
                var logs = await _auditLogService.GetReportingLogsAsync(filter);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get logs by entity type and ID
        /// </summary>
        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<ActionResult<List<AuditLogDto>>> GetLogsByEntity(string entityType, string entityId)
        {
            try
            {
                var logs = await _auditLogService.GetLogsByEntityAsync(entityType, entityId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get logs by action type
        /// </summary>
        [HttpGet("action/{action}")]
        public async Task<ActionResult<List<AuditLogDto>>> GetLogsByAction(string action)
        {
            try
            {
                var logs = await _auditLogService.GetLogsByActionAsync(action);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get logs by user
        /// </summary>
        [HttpGet("user/{username}")]
        public async Task<ActionResult<List<AuditLogDto>>> GetLogsByUser(string username)
        {
            try
            {
                var logs = await _auditLogService.GetLogsByUserAsync(username);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new audit log entry
        /// </summary>
        [HttpPost]
        [AllowAnonymous]  // Backend can log without auth
        public async Task<ActionResult<AuditLogDto>> CreateLog([FromBody] AuditLogDto logDto)
        {
            try
            {
                // Get IP address from request
                var ipAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                logDto.IpAddress = ipAddress;

                // Ensure timestamp is set
                if (logDto.Timestamp == default)
                {
                    logDto.Timestamp = DateTime.UtcNow;
                }

                var createdLog = await _auditLogService.CreateLogAsync(logDto);
                return Ok(createdLog);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
