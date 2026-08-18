using Shared.Domain;

namespace App.Domain.Notifications.Entities;

/// <summary>
/// إعدادات التنبيه — قابلة للتخصيص لكل مستخدم
/// </summary>
public sealed class ReminderSetting : Entity
{
    public string UserId { get; private set; } = null!;
    public int DaysBeforeHearing { get; private set; }
    public bool NotifyBySystem { get; private set; }
    public bool NotifyByEmail { get; private set; }
    public bool NotifyByWhatsApp { get; private set; }

    private ReminderSetting() { }

    private ReminderSetting(Guid id, string userId, int daysBeforeHearing,
        bool notifyBySystem, bool notifyByEmail, bool notifyByWhatsApp)
        : base(id)
    {
        UserId = userId;
        DaysBeforeHearing = daysBeforeHearing;
        NotifyBySystem = notifyBySystem;
        NotifyByEmail = notifyByEmail;
        NotifyByWhatsApp = notifyByWhatsApp;
    }

    public static ReminderSetting CreateDefault(string userId)
    {
        return new ReminderSetting(Guid.NewGuid(), userId, 1, true, true, false);
    }

    public static ReminderSetting Create(string userId, int daysBeforeHearing,
        bool notifyBySystem, bool notifyByEmail, bool notifyByWhatsApp)
    {
        return new ReminderSetting(Guid.NewGuid(), userId, daysBeforeHearing,
            notifyBySystem, notifyByEmail, notifyByWhatsApp);
    }

    public void Update(int daysBeforeHearing, bool notifyBySystem, bool notifyByEmail, bool notifyByWhatsApp)
    {
        DaysBeforeHearing = daysBeforeHearing;
        NotifyBySystem = notifyBySystem;
        NotifyByEmail = notifyByEmail;
        NotifyByWhatsApp = notifyByWhatsApp;
    }
}
