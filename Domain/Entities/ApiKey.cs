using Domain.Common;

namespace Domain.Entities
{

    /// <summary>
    /// Entité représentant une clé API pour intégrations SI
    /// </summary>
    public class ApiKey : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string KeyHash { get; set; } = string.Empty; // Hash de la clé, jamais la clé en clair
        public string? Description { get; set; }

        // Permissions (JSON array)
        public string Permissions { get; set; } = "[]";

        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Limites de taux
        public int? RateLimitPerMinute { get; set; }
        public int? RateLimitPerDay { get; set; }

        // Statistiques
        public long TotalCalls { get; set; } = 0;
        public DateTime? LastUsedAt { get; set; }

        // Propriétaire
        public Guid? OwnerId { get; set; }
        public virtual User? Owner { get; set; }

        public bool IsValid => IsActive && (!ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow);
    }
}
