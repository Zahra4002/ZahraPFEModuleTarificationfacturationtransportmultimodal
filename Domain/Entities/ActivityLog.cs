using Domain.Common;

namespace Domain.Entities
{

    /// <summary>
    /// Entité représentant un log d'activité utilisateur
    /// </summary>
    public class ActivityLog : Entity
    {
        public Guid? UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, LOGIN, etc.
        public string EntityType { get; set; } = string.Empty; // Nom de l'entité concernée
        public Guid? EntityId { get; set; }

        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
    }
}
