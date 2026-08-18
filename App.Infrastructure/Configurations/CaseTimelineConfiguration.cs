using App.Domain.Cases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CaseTimelineConfiguration : IEntityTypeConfiguration<CaseTimeline>
{
    public void Configure(EntityTypeBuilder<CaseTimeline> builder)
    {
        builder.ToTable("CaseTimelines");
        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Title).HasMaxLength(300).IsRequired();
        builder.Property(ct => ct.Description).HasMaxLength(2000);
        builder.Property(ct => ct.CreatedBy).HasMaxLength(450).IsRequired();

        builder.HasOne(ct => ct.Case)
            .WithMany(c => c.CaseTimelines)
            .HasForeignKey(ct => ct.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ct => ct.CaseId);
        builder.HasIndex(ct => ct.CreatedAt);
    }
}
