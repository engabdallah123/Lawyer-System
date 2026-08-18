using App.Domain.Notifications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations;

public class ReminderSettingConfiguration : IEntityTypeConfiguration<ReminderSetting>
{
    public void Configure(EntityTypeBuilder<ReminderSetting> builder)
    {
        builder.ToTable("ReminderSettings");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId).HasMaxLength(450).IsRequired();

        builder.HasIndex(r => r.UserId).IsUnique();
    }
}
