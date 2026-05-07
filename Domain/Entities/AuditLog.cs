// Domain/Entities/AuditLog.cs
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class AuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Type of action: CREATE, UPDATE, DELETE, APPROVE, REJECT, VIEW, EXPORT, OTHER
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// Human-readable description of the action
        /// </summary>
        public string ActionDescription { get; set; }
        
        /// <summary>
        /// Type of entity affected: Invoice, Quote, Contract, Client, Supplier, etc.
        /// </summary>
        public string EntityType { get; set; }
        
        /// <summary>
        /// ID of the entity that was affected
        /// </summary>
        public string EntityId { get; set; }
        
        /// <summary>
        /// Name/Label of the entity
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// Username of the user who performed the action
        /// </summary>
        public string PerformedBy { get; set; }
        
        /// <summary>
        /// Role of the user who performed the action
        /// </summary>
        public string PerformedByRole { get; set; }
        
        /// <summary>
        /// Result of the action: SUCCESS, FAILURE
        /// </summary>
        public string Result { get; set; } = "SUCCESS";
        
        /// <summary>
        /// Additional details about the action
        /// </summary>
        public string Details { get; set; }
        
        /// <summary>
        /// Changes made (serialized JSON with before/after values)
        /// </summary>
        public string Changes { get; set; }
        
        /// <summary>
        /// IP address of the user
        /// </summary>
        public string IpAddress { get; set; }
    }
}
