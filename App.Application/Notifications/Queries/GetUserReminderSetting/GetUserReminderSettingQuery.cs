using App.Application.Notifications.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Queries.GetUserReminderSetting
{
    public record GetUserReminderSettingQuery(string UserId) : IQuery<ReminderSettingDto>;
}
