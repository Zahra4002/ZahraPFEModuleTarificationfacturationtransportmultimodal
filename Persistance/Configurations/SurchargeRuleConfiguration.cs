// Persistance/Configurations/SurchargeRuleConfiguration.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class SurchargeRuleConfiguration : IEntityTypeConfiguration<SurchargeRule>
    {
        public void Configure(EntityTypeBuilder<SurchargeRule> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.ConditionsJson)
                .IsRequired()
                .HasColumnType("text"); // Pour stocker du JSON

            builder.Property(r => r.ApplicableTransportModes)
                .HasColumnType("text"); // Pour stocker du JSON

            builder.Property(r => r.OverrideValue)
                .HasPrecision(18, 2);

            builder.Property(r => r.Priority)
                .HasDefaultValue(0);

            builder.Property(r => r.IsActive)
                .HasDefaultValue(true);

            // Relations avec Zone
            builder.HasOne(r => r.ZoneFrom)
                .WithMany()
                .HasForeignKey(r => r.ZoneFromId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ZoneTo)
                .WithMany()
                .HasForeignKey(r => r.ZoneToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation avec Surcharge
            builder.HasOne(r => r.Surcharge)
                .WithMany(s => s.Rules)
                .HasForeignKey(r => r.SurchargeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}