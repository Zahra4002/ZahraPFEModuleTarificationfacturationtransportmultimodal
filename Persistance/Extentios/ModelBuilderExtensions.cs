using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Extentios
{
    public static class ModelBuilderExtensions
    {
        public static void SeedSMSContext(this ModelBuilder modelBuilder)
        {
            SeedCurrency(modelBuilder);
            SeedZone(modelBuilder);
            SeedMerchandiseType(modelBuilder);
            SeedSupplier(modelBuilder);
            SeedTaxRules(modelBuilder);
        }

        private static void SeedCurrency(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Code = "EUR",
                    Name = "Euro",
                    Symbol = "€",
                    DecimalPlaces = 2,
                    IsActive = true,
                    IsDefault = true
                },
                new Currency
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    Code = "USD",
                    Name = "Dollar américain",
                    Symbol = "$",
                    DecimalPlaces = 2,
                    IsActive = true,
                    IsDefault = false
                },
                new Currency
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    Code = "TND",
                    Name = "Dinar tunisien",
                    Symbol = "د.ت",
                    DecimalPlaces = 3,
                    IsActive = true,
                    IsDefault = false
                },
                new Currency
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    Code = "GBP",
                    Name = "Livre sterling",
                    Symbol = "£",
                    DecimalPlaces = 2,
                    IsActive = true,
                    IsDefault = false
                }
            );
        }

        private static void SeedZone(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Zone>().HasData(
                new Zone
                {
                    Id = new Guid("51111111-1111-1111-1111-111111111111"),
                    Code = "DZA",
                    Name = "Algérie",
                    Country = "Algérie",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("61111111-1111-1111-1111-111111111111"),
                    Code = "DEU",
                    Name = "Allemagne",
                    Country = "Allemagne",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("71111111-1111-1111-1111-111111111111"),
                    Code = "CHN",
                    Name = "Chine",
                    Country = "Chine",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("81111111-1111-1111-1111-111111111111"),
                    Code = "ESP",
                    Name = "Espagne",
                    Country = "Espagne",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("91111111-1111-1111-1111-111111111111"),
                    Code = "USA",
                    Name = "États-Unis",
                    Country = "États-Unis",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("10111111-1111-1111-1111-111111111111"),
                    Code = "FRA",
                    Name = "France",
                    Country = "France",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("12111111-1111-1111-1111-111111111111"),
                    Code = "ITA",
                    Name = "Italie",
                    Country = "Italie",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("13111111-1111-1111-1111-111111111111"),
                    Code = "LBY",
                    Name = "Libye",
                    Country = "Libye",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("14111111-1111-1111-1111-111111111111"),
                    Code = "MAR",
                    Name = "Maroc",
                    Country = "Maroc",
                    Region = null,
                    IsActive = true
                }
            );
        }

        private static void SeedMerchandiseType(ModelBuilder modelBuilder)
        {
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
        }

        private static void SeedSupplier(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = "DUMMY",
                    Name = "Unknown Supplier",
                    IsActive = true,
                    Address = "N/A",
                    DefaultCurrencyCode = "USD"
                }
            );
        }

        private static void SeedTaxRules(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxRule>().HasData(
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                    Code = "VAT-FR-20",
                    Name = "TVA France 20%",
                    Country = "FR",
                    Region = null,
                    StandardRate = 20.0m,
                    ReducedRate = 5.5m,
                    SuperReducedRate = 2.1m,
                    ZeroRate = 0m,
                    AllowExemption = false,
                    ExemptionConditions = null,
                    SurchargeId = null,
                    ValidFrom = new DateTime(2024, 1, 1),
                    ValidTo = null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                },
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
                    Code = "VAT-FR-55",
                    Name = "TVA France 5.5% (réduit)",
                    Country = "FR",
                    Region = null,
                    StandardRate = 20.0m,
                    ReducedRate = 5.5m,
                    SuperReducedRate = 2.1m,
                    ZeroRate = 0m,
                    AllowExemption = false,
                    ExemptionConditions = null,
                    SurchargeId = null,
                    ValidFrom = new DateTime(2024, 1, 1),
                    ValidTo = null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                },
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
                    Code = "VAT-FR-21",
                    Name = "TVA France 2.1% (super réduit)",
                    Country = "FR",
                    Region = null,
                    StandardRate = 20.0m,
                    ReducedRate = 5.5m,
                    SuperReducedRate = 2.1m,
                    ZeroRate = 0m,
                    AllowExemption = false,
                    ExemptionConditions = null,
                    SurchargeId = null,
                    ValidFrom = new DateTime(2024, 1, 1),
                    ValidTo = null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                },
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
                    Code = "VAT-TN-19",
                    Name = "TVA Tunisie 19%",
                    Country = "TN",
                    Region = null,
                    StandardRate = 19.0m,
                    ReducedRate = 7.0m,
                    SuperReducedRate = null,
                    ZeroRate = 0m,
                    AllowExemption = true,
                    ExemptionConditions = "{\"export\": true, \"sectors\": [\"agriculture\", \"education\"]}",
                    SurchargeId = null,
                    ValidFrom = new DateTime(2024, 1, 1),
                    ValidTo = null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                },
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"),
                    Code = "VAT-TN-07",
                    Name = "TVA Tunisie 7% (réduit)",
                    Country = "TN",
                    Region = null,
                    StandardRate = 19.0m,
                    ReducedRate = 7.0m,
                    SuperReducedRate = null,
                    ZeroRate = 0m,
                    AllowExemption = true,
                    ExemptionConditions = "{\"sectors\": [\"hotels\", \"restaurants\"]}",
                    SurchargeId = null,
                    ValidFrom = new DateTime(2024, 1, 1),
                    ValidTo = null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                },
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"),
                    Code = "VAT-FR-196",
                    Name = "Ancien taux TVA France 19.6%",
                    Country = "FR",
                    Region = null,
                    StandardRate = 19.6m,
                    ReducedRate = 5.5m,
                    SuperReducedRate = 2.1m,
                    ZeroRate = 0m,
                    AllowExemption = false,
                    ExemptionConditions = null,
                    SurchargeId = null,
                    ValidFrom = new DateTime(2000, 1, 1),
                    ValidTo = new DateTime(2013, 12, 31),
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                },
                new TaxRule
                {
                    Id = Guid.Parse("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"),
                    Code = "VAT-EXPORT",
                    Name = "Exportation hors UE",
                    Country = "FR",
                    Region = null,
                    StandardRate = 20.0m,
                    ReducedRate = null,
                    SuperReducedRate = null,
                    ZeroRate = 0m,
                    AllowExemption = true,
                    ExemptionConditions = "{\"proofOfExport\": true, \"documents\": [\"EX1\", \"invoice\"]}",
                    SurchargeId = null,
                    ValidFrom = new DateTime(2024, 1, 1),
                    ValidTo = null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    IsDeleted = false
                }
            );
        }
    }
}
