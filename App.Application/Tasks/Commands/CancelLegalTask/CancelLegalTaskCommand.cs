using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.CancelLegalTask
{
    public record CancelLegalTaskCommand(Guid Id, string UpdatedBy) : ICommand;
}
