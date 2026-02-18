using Domain.Common;

namespace Domain.Entities
{
    public class WebhookConfig : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty; // Pour signature HMAC

        // Événements auxquels s'abonner (JSON array)
        public string SubscribedEvents { get; set; } = "[]";

        // En-têtes personnalisés (JSON object)
        public string? CustomHeaders { get; set; }

        public bool IsActive { get; set; } = true;
        public int MaxRetries { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 60;

        // Statistiques
        public int TotalCalls { get; set; } = 0;
        public int SuccessfulCalls { get; set; } = 0;
        public int FailedCalls { get; set; } = 0;
        public DateTime? LastCallAt { get; set; }
        public bool? LastCallSuccess { get; set; }

        // Navigation properties
        public virtual ICollection<WebhookLog> Logs { get; set; } = new List<WebhookLog>();
    }
}
