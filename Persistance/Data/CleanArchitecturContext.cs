using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Configurations;

namespace Persistance.Data
{
    public class CleanArchitecturContext : DbContextBase, ICleanArchitecturContext
    {
        public CleanArchitecturContext(DbContextOptions options) : base(options) { }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Domain.Entities.User> Users { get; set; }

        public DbSet<Zone> Zones { get; set; }
        public DbSet<TariffLine> TariffLines { get; set; }
        public DbSet<Client> Clients { get; set; }

        public DbSet<Quote> Quotes { get; set; }          
        public DbSet<Shipment> Shipments { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ContractPricing> ContractPricings { get; set; }
        public DbSet<TransportSegment> TransportSegments { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<TaxRule> TaxRules { get; set; }
        public DbSet<Surcharge> Surcharges { get; set; }
        public DbSet<SurchargeRule> SurchargeRules { get; set; }

        public DbSet<MerchandiseType> MerchandiseTypes { get; set; }

        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<WebhookConfig> WebhookConfigs { get; set; }
        public DbSet<WebhookLog> WebhookLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed data pour Currencies avec GUID
            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Code = "EUR",
                    Name = "Euro",
                    Symbol = "€",
                    IsActive = true
                },
                new Currency
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Code = "USD",
                    Name = "US Dollar",
                    Symbol = "$",
                    IsActive = true
                },
                new Currency
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Code = "MAD",
                    Name = "Dirham Marocain",
                    Symbol = "DH",
                    IsActive = true
                }
            );



            // modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new SurchargeConfiguration());
            modelBuilder.ApplyConfiguration(new SurchargeRuleConfiguration());


            modelBuilder.Entity<ExchangeRate>()
                .HasOne(er => er.FromCurrency)
                .WithMany(c => c.ExchangeRatesFrom)
                .HasForeignKey(er => er.FromCurrencyId);

            modelBuilder.Entity<TariffLine>(entity =>
            {
                entity.HasOne(t => t.ZoneFrom)
                    .WithMany(z => z.TariffLinesFrom)
                    .HasForeignKey(t => t.ZoneFromId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ZoneTo)
                    .WithMany(z => z.TariffLinesTo)
                    .HasForeignKey(t => t.ZoneToId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Client addresses
            modelBuilder.Entity<Client>(entity =>
            {
                entity.OwnsOne(c => c.BullingAddress, a =>
                {
                    a.WithOwner();
                    a.Property(p => p.Street).HasColumnName("Billing_Street");
                    a.Property(p => p.City).HasColumnName("Billing_City");
                });
                entity.Navigation(c => c.BullingAddress).IsRequired();

                entity.OwnsOne(c => c.ShippingAddress, a =>
                {
                    a.WithOwner();
                    a.Property(p => p.Street).HasColumnName("Shipping_Street");
                    a.Property(p => p.City).HasColumnName("Shipping_City");
                });
                entity.Navigation(c => c.ShippingAddress).IsRequired();
            });

            // Quote addresses
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.OwnsOne(q => q.OriginAddress, a =>
                {
                    a.WithOwner();
                    a.Property(p => p.Street).HasColumnName("Quote_Origin_Street");
                    a.Property(p => p.City).HasColumnName("Quote_Origin_City");
                });
                entity.Navigation(q => q.OriginAddress).IsRequired();

                entity.OwnsOne(q => q.DestinationAddress, a =>
                {
                    a.WithOwner();
                    a.Property(p => p.Street).HasColumnName("Quote_Dest_Street");
                    a.Property(p => p.City).HasColumnName("Quote_Dest_City");
                });
                entity.Navigation(q => q.DestinationAddress).IsRequired();
            });
            // MerchandiseType 
            modelBuilder.Entity<MerchandiseType>().HasData(
                new MerchandiseType
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Code = "GEN001",
                    Name = "General Cargo",
                    Description = "Standard general cargo with no special requirements",
                    HazardousLevel = 0,
                    PriceMultiplier = 1.0m,
                    RequiresSpecialHandling = false,
                    IsActive = true
                },
                new MerchandiseType
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Code = "PER002",
                    Name = "Perishable Goods",
                    Description = "Temperature-sensitive goods requiring refrigeration or special handling",
                    HazardousLevel = 0,
                    PriceMultiplier = 1.5m,
                    RequiresSpecialHandling = true,
                    IsActive = true
                },
                new MerchandiseType
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Code = "HAZ003",
                    Name = "Hazardous Materials",
                    Description = "Dangerous goods requiring special permits and handling procedures",
                    HazardousLevel = 3,
                    PriceMultiplier = 2.0m,
                    RequiresSpecialHandling = true,
                    IsActive = true
                }
            );

            // Shipment addresses
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.OwnsOne(s => s.OriginAddress, a =>
                {
                    a.WithOwner();
                    a.Property(p => p.Street).HasColumnName("Shipment_Origin_Street");
                    a.Property(p => p.City).HasColumnName("Shipment_Origin_City");
                });
                entity.Navigation(s => s.OriginAddress).IsRequired();

                entity.OwnsOne(s => s.DestinationAddress, a =>
                {
                    a.WithOwner();
                    a.Property(p => p.Street).HasColumnName("Shipment_Dest_Street");
                    a.Property(p => p.City).HasColumnName("Shipment_Dest_City");
                });
                entity.Navigation(s => s.DestinationAddress).IsRequired();
            });
        }
    }
}
