using App.Application.Dashboard.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Dashboard.Queries.GetOwnerDashboard
{
    public record GetOwnerDashboardQuery() : IQuery<OwnerDashboardDto>;
}
