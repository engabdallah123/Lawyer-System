using App.Domain.PowerOfAttorney.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class PowerOfAttorneyConfiguration : IEntityTypeConfiguration<PowerOfAttorney>
{
    public void Configure(EntityTypeBuilder<PowerOfAttorney> builder)
    {
        builder.ToTable("PowerOfAttorneys");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PowerNumber).HasMaxLength(50).IsRequired();
        builder.Property(p => p.NotaryName).HasMaxLength(200);
        builder.Property(p => p.NotaryNumber).HasMaxLength(50);
        builder.Property(p => p.FilePath).HasMaxLength(500);
        builder.Property(p => p.Notes).HasMaxLength(2000);
        builder.Property(p => p.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(p => p.UpdatedBy).HasMaxLength(450);

        builder.HasOne(p => p.Client)
            .WithMany(c => c.PowerOfAttorneys)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Case)
            .WithMany(c => c.PowerOfAttorneys)
            .HasForeignKey(p => p.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.ClientId);
        builder.HasIndex(p => p.CaseId);
        builder.HasIndex(p => p.PowerNumber);
        builder.HasIndex(p => p.ExpiryDate).HasFilter("[ExpiryDate] IS NOT NULL");
        builder.HasIndex(p => p.IsActive);
    }
}
