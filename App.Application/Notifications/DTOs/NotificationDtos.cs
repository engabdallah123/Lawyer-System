using App.Domain.Notifications.Enums;

namespace App.Application.Notifications.DTOs;

public record NotificationDto(
    Guid Id,
    string UserId,
    string Title,
    string Message,
    NotificationType Type,
    string TypeName,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt);

public record ReminderSettingDto(
    Guid Id,
    string UserId,
    int DaysBeforeHearing,
    bool NotifyBySystem,
    bool NotifyByEmail,
    bool NotifyByWhatsApp);
