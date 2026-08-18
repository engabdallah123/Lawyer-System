using App.Domain.Consultations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("Consultations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Subject).HasMaxLength(300).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(4000);
        builder.Property(c => c.Fee).HasPrecision(18, 2);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(c => c.UpdatedBy).HasMaxLength(450);

        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Consultations)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.ClientId);
        builder.HasIndex(c => c.ConsultationDate);
        builder.HasIndex(c => c.Status);
    }
}
