using App.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.ToTable("DocumentTypes");
        builder.HasKey(dt => dt.Id);
        builder.Property(dt => dt.Id).ValueGeneratedOnAdd();

        builder.Property(dt => dt.Name).HasMaxLength(100).IsRequired();
        builder.Property(dt => dt.Description).HasMaxLength(500);

        builder.HasIndex(dt => dt.Name);
        builder.HasIndex(dt => dt.IsActive);
    }
}
