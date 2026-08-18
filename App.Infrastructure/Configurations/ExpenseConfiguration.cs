using App.Domain.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExpenseType).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.ReceiptPath).HasMaxLength(500);
        builder.Property(e => e.PaidBy).HasMaxLength(450).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(450);

        builder.HasOne(e => e.Case)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.CaseId);
        builder.HasIndex(e => e.ExpenseDate);
    }
}
