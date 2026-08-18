using System;
using System.Collections.Generic;
using System.Text;
using Shared.Application.Messaging;

namespace App.Application.Documents.Commands.DeleteDocument
{
    public record DeleteDocumentCommand(Guid Id, string DeletedBy) : ICommand;
}
 