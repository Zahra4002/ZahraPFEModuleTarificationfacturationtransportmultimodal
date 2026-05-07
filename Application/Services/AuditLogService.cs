// Application/Services/AuditLogService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly IReadOnlyRepository<AuditLog> _auditLogReadOnlyRepository;

        public AuditLogService(
            IRepository<AuditLog> auditLogRepository,
            IReadOnlyRepository<AuditLog> auditLogReadOnlyRepository)
        {
            _auditLogRepository = auditLogRepository;
            _auditLogReadOnlyRepository = auditLogReadOnlyRepository;
        }

        public async Task<List<AuditLogDto>> SearchLogsAsync(AuditLogFilterDto filter)
        {
            // Get all logs as enumerable
            var allLogs = await _auditLogReadOnlyRepository.GetAllAsync(CancellationToken.None);
            var query = allLogs.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(l => l.Timestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.Timestamp <= filter.EndDate.Value);

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(l => l.Action == filter.Action);

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(l => l.EntityType == filter.EntityType);

            if (!string.IsNullOrEmpty(filter.PerformedBy))
                query = query.Where(l => l.PerformedBy.Contains(filter.PerformedBy));

            if (!string.IsNullOrEmpty(filter.Result))
                query = query.Where(l => l.Result == filter.Result);

            var logs = query
                .OrderByDescending(l => l.Timestamp)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetLogsByEntityAsync(string entityType, string entityId)
        {
            var allLogs = await _auditLogReadOnlyRepository.GetAllAsync(CancellationToken.None);
            var logs = allLogs
                .Where(l => l.EntityType == entityType && l.EntityId == entityId)
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetLogsByActionAsync(string action)
        {
            var allLogs = await _auditLogReadOnlyRepository.GetAllAsync(CancellationToken.None);
            var logs = allLogs
                .Where(l => l.Action == action)
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToList();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetLogsByUserAsync(string username)
        {
            var allLogs = await _auditLogReadOnlyRepository.GetAllAsync(CancellationToken.None);
            var logs = allLogs
                .Where(l => l.PerformedBy == username)
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToList();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetReportingLogsAsync(AuditLogFilterDto filter)
        {
            var allLogs = await _auditLogReadOnlyRepository.GetAllAsync(CancellationToken.None);
            var query = allLogs.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(l => l.Timestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.Timestamp <= filter.EndDate.Value);

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(l => l.Action == filter.Action);

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(l => l.EntityType == filter.EntityType);

            if (!string.IsNullOrEmpty(filter.PerformedBy))
                query = query.Where(l => l.PerformedBy.Contains(filter.PerformedBy));

            if (!string.IsNullOrEmpty(filter.Result))
                query = query.Where(l => l.Result == filter.Result);

            var logs = query
                .OrderByDescending(l => l.Timestamp)
                .Take(1000)  // Last 1000 logs for reporting
                .ToList();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<AuditLogDto> CreateLogAsync(AuditLogDto logDto)
        {
            var auditLog = new AuditLog
            {
                Id = logDto.Id ?? Guid.NewGuid().ToString(),
                Timestamp = logDto.Timestamp == default ? DateTime.UtcNow : logDto.Timestamp,
                Action = logDto.Action,
                ActionDescription = logDto.ActionDescription,
                EntityType = logDto.EntityType,
                EntityId = logDto.EntityId,
                EntityName = logDto.EntityName,
                PerformedBy = logDto.PerformedBy,
                PerformedByRole = logDto.PerformedByRole,
                Result = logDto.Result ?? "SUCCESS",
                Details = logDto.Details,
                Changes = logDto.Changes != null ? JsonSerializer.Serialize(logDto.Changes) : null,
                IpAddress = logDto.IpAddress
            };

            await _auditLogRepository.AddAsync(auditLog, CancellationToken.None);
            return MapToDto(auditLog);
        }

        public async Task LogCreateAsync(string entityType, string entityId, string entityName, string performedBy, string role, string details = null)
        {
            await CreateLogAsync(new AuditLogDto
            {
                Action = "CREATE",
                ActionDescription = $"{entityType} créé",
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                PerformedBy = performedBy,
                PerformedByRole = role,
                Result = "SUCCESS",
                Details = details
            });
        }

        public async Task LogUpdateAsync(string entityType, string entityId, string entityName, string performedBy, string role, Dictionary<string, object> changes = null, string details = null)
        {
            await CreateLogAsync(new AuditLogDto
            {
                Action = "UPDATE",
                ActionDescription = $"{entityType} modifié",
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                PerformedBy = performedBy,
                PerformedByRole = role,
                Result = "SUCCESS",
                Changes = changes,
                Details = details
            });
        }

        public async Task LogDeleteAsync(string entityType, string entityId, string entityName, string performedBy, string role, string details = null)
        {
            await CreateLogAsync(new AuditLogDto
            {
                Action = "DELETE",
                ActionDescription = $"{entityType} supprimé",
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                PerformedBy = performedBy,
                PerformedByRole = role,
                Result = "SUCCESS",
                Details = details
            });
        }

        public async Task LogApprovalAsync(string entityType, string entityId, string entityName, bool approved, string performedBy, string role, string reason = null)
        {
            await CreateLogAsync(new AuditLogDto
            {
                Action = approved ? "APPROVE" : "REJECT",
                ActionDescription = approved ? $"{entityType} approuvé" : $"{entityType} rejeté",
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                PerformedBy = performedBy,
                PerformedByRole = role,
                Result = "SUCCESS",
                Details = reason
            });
        }

        private AuditLogDto MapToDto(AuditLog log)
        {
            var dto = new AuditLogDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Action = log.Action,
                ActionDescription = log.ActionDescription,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                EntityName = log.EntityName,
                PerformedBy = log.PerformedBy,
                PerformedByRole = log.PerformedByRole,
                Result = log.Result,
                Details = log.Details,
                IpAddress = log.IpAddress
            };

            if (!string.IsNullOrEmpty(log.Changes))
            {
                try
                {
                    dto.Changes = JsonSerializer.Deserialize<Dictionary<string, object>>(log.Changes);
                }
                catch { }
            }

            return dto;
        }
    }
}