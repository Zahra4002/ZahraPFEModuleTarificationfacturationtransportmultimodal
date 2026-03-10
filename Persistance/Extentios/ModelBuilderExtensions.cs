using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Extentios
{
    public static class ModelBuilderExtensions
    {
        public static void SeedSMSContext(this ModelBuilder modelBuilder)
        {
            SeedCurrency(modelBuilder); // devies
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
