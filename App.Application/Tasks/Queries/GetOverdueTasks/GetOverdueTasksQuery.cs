using App.Application.Tasks.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Queries.GetOverdueTasks
{
    public record GetOverdueTasksQuery(string? UserId = null) : IQuery<IEnumerable<LegalTaskDto>>;
}
