using App.Domain.Cases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CasePartyConfiguration : IEntityTypeConfiguration<CaseParty>
{
    public void Configure(EntityTypeBuilder<CaseParty> builder)
    {
        builder.ToTable("CaseParties");
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.PartyName).HasMaxLength(200);
        builder.Property(cp => cp.PartyType).HasMaxLength(50);
        builder.Property(cp => cp.PartyRole).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(cp => cp.Address).HasMaxLength(300);
        builder.Property(cp => cp.Phone).HasMaxLength(30);
        builder.Property(cp => cp.Email).HasMaxLength(100);
        builder.Property(cp => cp.LawyerName).HasMaxLength(200);
        builder.Property(cp => cp.LawyerPhone).HasMaxLength(30);
        builder.Property(cp => cp.Notes).HasMaxLength(1000);

        builder.HasOne(cp => cp.Case)
            .WithMany(c => c.CaseParties)
            .HasForeignKey(cp => cp.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.Client)
            .WithMany(c => c.CaseParties)
            .HasForeignKey(cp => cp.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(cp => cp.CaseId);
        builder.HasIndex(cp => cp.ClientId);
    }
}
