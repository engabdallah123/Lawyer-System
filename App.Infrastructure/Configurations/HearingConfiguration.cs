using App.Domain.Hearings.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class HearingConfiguration : IEntityTypeConfiguration<Hearing>
{
    public void Configure(EntityTypeBuilder<Hearing> builder)
    {
        builder.ToTable("Hearings");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.HearingType).HasMaxLength(100).IsRequired();
        builder.Property(h => h.Result).HasMaxLength(2000);
        builder.Property(h => h.Notes).HasMaxLength(2000);
        builder.Property(h => h.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(450);

        builder.HasOne(h => h.Case)
            .WithMany(c => c.Hearings)
            .HasForeignKey(h => h.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => h.CaseId);
        builder.HasIndex(h => h.HearingDate);
        builder.HasIndex(h => h.NextHearingDate).HasFilter("[NextHearingDate] IS NOT NULL");
    }
}
