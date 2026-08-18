using App.Domain.Tasks.Enums;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.UpdateLegalTask
{
    public record UpdateLegalTaskCommand(
      Guid Id,
      string Title,
      string? Description,
      DateTime? DueDate,
      TaskPriority Priority,
      string UpdatedBy) : ICommand;
}
