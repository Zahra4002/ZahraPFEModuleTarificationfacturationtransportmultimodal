using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Extentios
{
    public static class ModelBuilderExtensions
    {
        public static void SeedSMSContext(this ModelBuilder modelBuilder)
        {
            SeedCurrency(modelBuilder); // devies
            SeedZone(modelBuilder); // Zone
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
                    Id = new Guid("1011111-1111-1111-1111-111111111111"),
                    Code = "FRA",
                    Name = "France",
                    Country = "France",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("1211111-1111-1111-1111-111111111111"),
                    Code = "ITA",
                    Name = "Italie",
                    Country = "Italie",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("1311111-1111-1111-1111-111111111111"),
                    Code = "LBY",
                    Name = "Libye",
                    Country = "Libye",
                    Region = null,
                    IsActive = true
                },
                new Zone
                {
                    Id = new Guid("1411111-1111-1111-1111-111111111111"),
                    Code = "MAR",
                    Name = "Maroc",
                    Country = "Maroc",
                    Region = null,
                    IsActive = true
                }
            );
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
    }
}
