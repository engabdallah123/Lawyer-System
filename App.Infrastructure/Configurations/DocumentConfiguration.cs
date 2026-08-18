using App.Domain.Documents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).HasMaxLength(300).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(2000);
        builder.Property(d => d.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(d => d.UpdatedBy).HasMaxLength(450);
        builder.Property(d => d.DeletedBy).HasMaxLength(450);

        builder.HasOne(d => d.Case)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Client)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.DocumentType)
            .WithMany(dt => dt.Documents)
            .HasForeignKey(d => d.DocumentTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        // CurrentVersion — self-referencing FK (optional)
        builder.HasOne(d => d.CurrentVersion)
            .WithOne()
            .HasForeignKey<Document>(d => d.CurrentVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.DocumentTypeId);

        builder.HasIndex(d => d.CaseId);
        builder.HasIndex(d => d.ClientId);
        builder.HasIndex(d => d.Name);
        builder.HasIndex(d => d.IsDeleted);
    }
}
