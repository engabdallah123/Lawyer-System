using App.Application.Notifications.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Queries.GetUserNotifications
{
    public record GetUserNotificationsQuery(
       string? UserId = null,
       bool? OnlyUnread = null,
       int Page = 1,
       int PageSize = 20) : IQuery<IEnumerable<NotificationDto>>;
}
