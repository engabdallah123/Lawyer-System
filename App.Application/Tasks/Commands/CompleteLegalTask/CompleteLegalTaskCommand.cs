using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.CompleteLegalTask
{
    public record CompleteLegalTaskCommand(Guid Id, string UpdatedBy) : ICommand;
}
