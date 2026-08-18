using App.Domain.Tasks.Enums;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.CreateLegalTask
{
    public record CreateLegalTaskCommand(
        Guid? CaseId,
        string AssignedToUserId,
        string Title,
        string? Description,
        DateTime? DueDate,
        TaskPriority Priority,
        string CreatedBy) : ICommand<Guid>;
}
