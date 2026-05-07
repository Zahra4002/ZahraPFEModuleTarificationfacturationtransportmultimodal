// Application/Interfaces/IAuditLogService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Search logs with optional filters
        /// </summary>
        Task<List<AuditLogDto>> SearchLogsAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Get logs by entity type and ID
        /// </summary>
        Task<List<AuditLogDto>> GetLogsByEntityAsync(string entityType, string entityId);

        /// <summary>
        /// Get logs by action type
        /// </summary>
        Task<List<AuditLogDto>> GetLogsByActionAsync(string action);

        /// <summary>
        /// Get logs by user
        /// </summary>
        Task<List<AuditLogDto>> GetLogsByUserAsync(string username);

        /// <summary>
        /// Get logs for reporting (last 1000 entries sorted by date)
        /// </summary>
        Task<List<AuditLogDto>> GetReportingLogsAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Create new audit log entry
        /// </summary>
        Task<AuditLogDto> CreateLogAsync(AuditLogDto logDto);

        /// <summary>
        /// Log entity creation
        /// </summary>
        Task LogCreateAsync(string entityType, string entityId, string entityName, string performedBy, string role, string details = null);

        /// <summary>
        /// Log entity update
        /// </summary>
        Task LogUpdateAsync(string entityType, string entityId, string entityName, string performedBy, string role, Dictionary<string, object> changes = null, string details = null);

        /// <summary>
        /// Log entity deletion
        /// </summary>
        Task LogDeleteAsync(string entityType, string entityId, string entityName, string performedBy, string role, string details = null);

        /// <summary>
        /// Log approval/rejection
        /// </summary>
        Task LogApprovalAsync(string entityType, string entityId, string entityName, bool approved, string performedBy, string role, string reason = null);
    }
}
