using App.Domain.Tasks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class LegalTaskConfiguration : IEntityTypeConfiguration<LegalTask>
{
    public void Configure(EntityTypeBuilder<LegalTask> builder)
    {
        builder.ToTable("LegalTasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.AssignedToUserId).HasMaxLength(450).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(t => t.UpdatedBy).HasMaxLength(450);

        builder.HasOne(t => t.Case)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.CaseId);
        builder.HasIndex(t => t.AssignedToUserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.DueDate).HasFilter("[DueDate] IS NOT NULL");
        builder.HasIndex(t => t.Priority);
    }
}
