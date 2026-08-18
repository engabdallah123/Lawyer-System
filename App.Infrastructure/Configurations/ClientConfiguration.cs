using App.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ClientType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.FullName).HasMaxLength(200);
        builder.Property(c => c.CompanyName).HasMaxLength(300);
        builder.Property(c => c.NationalId).HasMaxLength(20);
        builder.Property(c => c.CommercialRegister).HasMaxLength(30);
        builder.Property(c => c.Phone).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Mobile).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.CreatedBy).HasMaxLength(450).IsRequired();
        builder.Property(c => c.UpdatedBy).HasMaxLength(450);
        builder.Property(c => c.DeletedBy).HasMaxLength(450);

        // Indexes
        builder.HasIndex(c => c.NationalId).HasFilter("[NationalId] IS NOT NULL");
        builder.HasIndex(c => c.CommercialRegister).HasFilter("[CommercialRegister] IS NOT NULL");
        builder.HasIndex(c => c.Phone);
        builder.HasIndex(c => c.FullName);
        builder.HasIndex(c => c.CompanyName);
        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => c.IsActive);
    }
}
