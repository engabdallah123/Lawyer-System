using App.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CaseStatusConfiguration : IEntityTypeConfiguration<CaseStatus>
{
    public void Configure(EntityTypeBuilder<CaseStatus> builder)
    {
        builder.ToTable("CaseStatuses");
        builder.HasKey(cs => cs.Id);
        builder.Property(cs => cs.Id).ValueGeneratedOnAdd();

        builder.Property(cs => cs.Name).HasMaxLength(100).IsRequired();
        builder.Property(cs => cs.Color).HasMaxLength(20);

        builder.HasIndex(cs => cs.Name);
        builder.HasIndex(cs => cs.IsActive);
    }
}
