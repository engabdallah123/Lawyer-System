using App.Domain.Notifications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.UserId).HasMaxLength(450).IsRequired();
        builder.Property(n => n.Title).HasMaxLength(300).IsRequired();
        builder.Property(n => n.Message).HasMaxLength(2000).IsRequired();
        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
        builder.HasIndex(n => n.CreatedAt);
        builder.HasIndex(n => new { n.UserId, n.IsRead });
    }
}
