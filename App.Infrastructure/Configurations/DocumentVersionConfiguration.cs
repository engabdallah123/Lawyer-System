using App.Domain.Documents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");
        builder.HasKey(dv => dv.Id);

        builder.Property(dv => dv.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(dv => dv.FileName).HasMaxLength(300).IsRequired();
        builder.Property(dv => dv.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(dv => dv.UploadedBy).HasMaxLength(450).IsRequired();
        builder.Property(dv => dv.Notes).HasMaxLength(1000);

        builder.HasOne(dv => dv.Document)
            .WithMany(d => d.DocumentVersions)
            .HasForeignKey(dv => dv.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(dv => dv.DocumentId);
        builder.HasIndex(dv => new { dv.DocumentId, dv.VersionNumber }).IsUnique();
    }
}
