using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.MarkAllNotificationsAsRead
{
    public record MarkAllNotificationsAsReadCommand(string UserId) : ICommand;
}
