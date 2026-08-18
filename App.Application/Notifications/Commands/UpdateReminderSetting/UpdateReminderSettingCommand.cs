using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.UpdateReminderSetting
{
    public record UpdateReminderSettingCommand(
     string UserId,
     int DaysBeforeHearing,
     bool NotifyBySystem,
     bool NotifyByEmail,
     bool NotifyByWhatsApp) : ICommand;
}
