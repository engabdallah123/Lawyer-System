using App.Application.Tasks.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Queries.GetTasksByCase
{
    public record GetTasksByCaseQuery(Guid CaseId) : IQuery<IEnumerable<LegalTaskDto>>;
}
