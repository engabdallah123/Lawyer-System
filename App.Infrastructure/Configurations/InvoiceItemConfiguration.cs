using App.Domain.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");
        builder.HasKey(ii => ii.Id);

        builder.Property(ii => ii.Description).HasMaxLength(500).IsRequired();
        builder.Property(ii => ii.Quantity).HasPrecision(18, 4).IsRequired();
        builder.Property(ii => ii.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(ii => ii.Total).HasPrecision(18, 2).IsRequired();

        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.InvoiceItems)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ii => ii.InvoiceId);
    }
}
