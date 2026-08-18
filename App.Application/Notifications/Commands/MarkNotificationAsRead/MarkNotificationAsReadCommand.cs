using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.MarkNotificationAsRead
{
    public record MarkNotificationAsReadCommand(Guid Id) : ICommand;
}
