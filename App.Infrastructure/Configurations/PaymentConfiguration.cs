using App.Domain.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.PaymentMethod).HasMaxLength(100).IsRequired();
        builder.Property(p => p.ReferenceNumber).HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(1000);
        builder.Property(p => p.ReceivedBy).HasMaxLength(450).IsRequired();

        builder.HasOne(p => p.Client)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Case)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.FeeAgreement)
            .WithMany(f => f.Payments)
            .HasForeignKey(p => p.FeeAgreementId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.ClientId);
        builder.HasIndex(p => p.CaseId);
        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => p.FeeAgreementId);
        builder.HasIndex(p => p.InvoiceId);
    }
}
