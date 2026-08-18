using App.Domain.Cases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CaseAssignmentConfiguration : IEntityTypeConfiguration<CaseAssignment>
{
    public void Configure(EntityTypeBuilder<CaseAssignment> builder)
    {
        builder.ToTable("CaseAssignments");
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.UserId).HasMaxLength(450).IsRequired();
        builder.Property(ca => ca.RoleInCase).HasMaxLength(100).IsRequired();
        builder.Property(ca => ca.Notes).HasMaxLength(1000);

        builder.HasOne(ca => ca.Case)
            .WithMany(c => c.CaseAssignments)
            .HasForeignKey(ca => ca.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ca => ca.CaseId);
        builder.HasIndex(ca => ca.UserId);
        builder.HasIndex(ca => new { ca.CaseId, ca.UserId });
    }
}
