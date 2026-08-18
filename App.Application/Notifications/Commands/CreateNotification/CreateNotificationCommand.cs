using App.Domain.Notifications.Enums;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.CreateNotification
{
    public record CreateNotificationCommand(
     string UserId,
     string Title,
     string Message,
     NotificationType Type) : ICommand<Guid>;
}
