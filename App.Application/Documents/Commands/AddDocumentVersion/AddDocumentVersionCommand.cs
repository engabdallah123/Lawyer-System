using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Commands.AddDocumentVersion
{

    public record AddDocumentVersionCommand(
        Guid DocumentId,
        string FilePath,
        string FileName,
        string ContentType,
        long FileSize,
        string? Notes,
        string UploadedBy) : ICommand<Guid>;
}
