using App.Domain.Cases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.InternalNumber).HasMaxLength(50).IsRequired();
        builder.Property(c => c.CourtNumber).HasMaxLength(50);
        builder.Property(c => c.Title).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Circuit).HasMaxLength(200);
        builder.Property(c => c.JudgeName).HasMaxLength(200);
        builder.Property(c => c.ClaimAmount).HasPrecision(18, 2);
        builder.Property(c => c.Description).HasMaxLength(4000);
        builder.Property(c => c.CurrentStage).HasMaxLength(200);
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(c => c.UpdatedBy).HasMaxLength(450);
        builder.Property(c => c.DeletedBy).HasMaxLength(450);

        // Relationships
        builder.HasOne(c => c.CaseType)
            .WithMany(ct => ct.Cases)
            .HasForeignKey(c => c.CaseTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CaseStatus)
            .WithMany(cs => cs.Cases)
            .HasForeignKey(c => c.CaseStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Court)
            .WithMany(ct => ct.Cases)
            .HasForeignKey(c => c.CourtId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(c => c.InternalNumber);
        builder.HasIndex(c => c.CourtNumber).HasFilter("[CourtNumber] IS NOT NULL");
        builder.HasIndex(c => c.Title);
        builder.HasIndex(c => c.CaseTypeId);
        builder.HasIndex(c => c.CaseStatusId);
        builder.HasIndex(c => c.CourtId);
        builder.HasIndex(c => c.OpenDate);
        builder.HasIndex(c => c.IsDeleted);
    }
}
