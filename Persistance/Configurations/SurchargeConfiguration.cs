// Persistance/Configurations/SurchargeConfiguration.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class SurchargeConfiguration : IEntityTypeConfiguration<Surcharge>
    {
        public void Configure(EntityTypeBuilder<Surcharge> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(s => s.Code)
                .IsUnique();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(500); // Ou .HasColumnType("text") si très long

            builder.Property(s => s.Type)
                .HasConversion<int>(); // Stocker l'enum comme int

            builder.Property(s => s.CalculationType)
                .HasConversion<int>(); // Stocker l'enum comme int

            builder.Property(s => s.Value)
                .HasPrecision(18, 2);

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            // Relations
            builder.HasMany(s => s.Rules)
                .WithOne(r => r.Surcharge)
                .HasForeignKey(r => r.SurchargeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}