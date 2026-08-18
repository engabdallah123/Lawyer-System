using App.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.ToTable("Courts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.City).HasMaxLength(100);

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.City).HasFilter("[City] IS NOT NULL");
        builder.HasIndex(c => c.IsActive);
    }
}
