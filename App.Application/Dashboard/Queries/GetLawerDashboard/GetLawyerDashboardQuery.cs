using App.Application.Dashboard.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Dashboard.Queries.GetLawerDashboard
{
    public record GetLawyerDashboardQuery(string LawyerUserId) : IQuery<LawyerDashboardDto>;
}
