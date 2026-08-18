using App.Domain.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class FeeAgreementConfiguration : IEntityTypeConfiguration<FeeAgreement>
{
    public void Configure(EntityTypeBuilder<FeeAgreement> builder)
    {
        builder.ToTable("FeeAgreements");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.AgreementType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(f => f.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(f => f.PaidAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(f => f.Description).HasMaxLength(2000);
        builder.Property(f => f.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(f => f.UpdatedBy).HasMaxLength(450);

        builder.HasOne(f => f.Client)
            .WithMany(c => c.FeeAgreements)
            .HasForeignKey(f => f.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Case)
            .WithMany(c => c.FeeAgreements)
            .HasForeignKey(f => f.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(f => f.ClientId);
        builder.HasIndex(f => f.CaseId);
    }
}
