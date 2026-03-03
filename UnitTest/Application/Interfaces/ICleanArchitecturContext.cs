using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface ICleanArchitecturContext : IContext
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<Domain.Entities.User> Users { get; set; }

        // Entités métier ajoutées
        public DbSet<Zone> Zones { get; set; }
        public DbSet<TariffLine> TariffLines { get; set; }
        public DbSet<Client> Clients { get; set; }


        public DbSet<Quote> Quotes { get; set; }          // ✅ AJOUTE ÇA
        public DbSet<Domain.Entities.Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ContractPricing> ContractPricings { get; set; }
        public DbSet<TransportSegment> TransportSegments { get; set; }
        public DbSet<Domain.Entities.Currency> Currencies { get; set; }




        public DbSet<Surcharge> Surcharges { get; set; }
        public DbSet<SurchargeRule> SurchargeRules { get; set; }
        // Intégration / audit / sécurité
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<WebhookConfig> WebhookConfigs { get; set; }
        public DbSet<WebhookLog> WebhookLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}
