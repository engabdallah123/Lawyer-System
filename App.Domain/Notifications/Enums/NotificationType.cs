namespace App.Domain.Notifications.Enums;

/// <summary>
/// نوع الإشعار
/// </summary>
public enum NotificationType
{
    /// <summary>إشعار داخل النظام</summary>
    System = 0,

    /// <summary>بريد إلكتروني</summary>
    Email = 1,

    /// <summary>واتساب</summary>
    WhatsApp = 2
}
