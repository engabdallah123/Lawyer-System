using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Queries.GetUnreadNotificationsCount
{
    public record GetUnreadNotificationsCountQuery(string UserId) : IQuery<int>;
}
