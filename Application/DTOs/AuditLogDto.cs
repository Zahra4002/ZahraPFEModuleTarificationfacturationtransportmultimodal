// Application/DTOs/AuditLogDto.cs
using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class AuditLogDto
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string ActionDescription { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string EntityName { get; set; }
        public string PerformedBy { get; set; }
        public string PerformedByRole { get; set; }
        public string Result { get; set; }
        public string Details { get; set; }
        public Dictionary<string, object> Changes { get; set; }
        public string IpAddress { get; set; }
    }

    public class AuditLogFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string PerformedBy { get; set; }
        public string Result { get; set; }
        public int PageSize { get; set; } = 50;
        public int PageNumber { get; set; } = 1;
    }
}
