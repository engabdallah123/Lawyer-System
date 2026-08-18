using App.Domain.Notifications.Enums;
using Shared.Domain;

namespace App.Domain.Notifications.Entities;

/// <summary>
/// الإشعار
/// </summary>
public sealed class Notification : Entity
{
    public string UserId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    private Notification(Guid id, string userId, string title, string message, NotificationType type)
        : base(id)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Notification> Create(string userId, string title, string message, NotificationType type)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Notification>.Failure(new Error("Notification.TitleRequired", "عنوان الإشعار مطلوب."));

        var notification = new Notification(Guid.NewGuid(), userId, title.Trim(), message.Trim(), type);
        return Result<Notification>.Success(notification);
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
    }
}
