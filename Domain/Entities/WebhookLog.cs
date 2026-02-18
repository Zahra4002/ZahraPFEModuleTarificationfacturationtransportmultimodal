using Domain.Common;

namespace Domain.Entities
{

    public class WebhookLog : Entity
    {
        public Guid WebhookConfigId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string PayloadJson { get; set; } = "{}";

        public DateTime SentAt { get; set; }
        public int HttpStatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public bool IsSuccess { get; set; }

        public int AttemptNumber { get; set; } = 1;
        public string? ErrorMessage { get; set; }
        public long DurationMs { get; set; }

        // Navigation properties
        public virtual WebhookConfig WebhookConfig { get; set; } = null!;
    }
}
