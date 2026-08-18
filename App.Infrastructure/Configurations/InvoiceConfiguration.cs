using App.Domain.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(i => i.SubTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.Discount).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.TaxAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.PaidAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(i => i.Notes).HasMaxLength(2000);
        builder.Property(i => i.QRCodePath).HasMaxLength(500);
        builder.Property(i => i.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(i => i.UpdatedBy).HasMaxLength(450);

        builder.HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Case)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.FeeAgreement)
            .WithMany(f => f.Invoices)
            .HasForeignKey(i => i.FeeAgreementId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.ClientId);
        builder.HasIndex(i => i.CaseId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.IssueDate);
        builder.HasIndex(i => i.DueDate).HasFilter("[DueDate] IS NOT NULL");
    }
}
