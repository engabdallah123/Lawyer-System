using App.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CaseTypeConfiguration : IEntityTypeConfiguration<CaseType>
{
    public void Configure(EntityTypeBuilder<CaseType> builder)
    {
        builder.ToTable("CaseTypes");
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Id).ValueGeneratedOnAdd();

        builder.Property(ct => ct.Name).HasMaxLength(100).IsRequired();
        builder.Property(ct => ct.Description).HasMaxLength(500);

        builder.HasIndex(ct => ct.Name);
        builder.HasIndex(ct => ct.IsActive);
    }
}
