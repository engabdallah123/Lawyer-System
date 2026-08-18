using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Commands.CreateDocument
{
    public record CreateDocumentCommand(
     Guid? CaseId,
     Guid? ClientId,
     int? DocumentTypeId,
     string Name,
     string? Description,
     string FilePath,
     string FileName,
     string ContentType,
     long FileSize,
     string? Notes,
     string UploadedBy) : ICommand<Guid>;
}
