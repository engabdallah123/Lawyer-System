using App.Application.Tasks.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Queries.GetTasksByUser
{
    public record GetTasksByUserQuery(
     string? UserId = null,
     int? Status = null,
     int Page = 1,
     int PageSize = 20) : IQuery<IEnumerable<LegalTaskDto>>;
}
