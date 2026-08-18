using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Documents.Entities;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Commands.CreateDocument
{
    internal sealed class CreateDocumentCommandHandler : ICommandHandler<CreateDocumentCommand, Guid>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public CreateDocumentCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            var docResult = Document.Create(
                request.CaseId,
                request.ClientId,
                request.DocumentTypeId,
                request.Name,
                request.Description);

            if (docResult.IsFailure)
                return Result<Guid>.Failure(docResult.Error);

            var document = docResult.Value!;
            document.SetCreated(request.UploadedBy);

            await _unitOfWork.Documents.AddAsync(document, cancellationToken);

            var versionResult = DocumentVersion.Create(
                document.Id,
                1,
                request.FilePath,
                request.FileName,
                request.ContentType,
                request.FileSize,
                request.UploadedBy,
                request.Notes);

            if (versionResult.IsFailure)
                return Result<Guid>.Failure(versionResult.Error);

            var version = versionResult.Value!;
            await _unitOfWork.DocumentVersions.AddAsync(version, cancellationToken);

            document.SetCurrentVersion(version.Id);

            if (request.CaseId.HasValue)
            {
                var timelineResult = CaseTimeline.Create(
                    request.CaseId.Value,
                    "رفع مستند جديد",
                    $"تم رفع المستند '{request.Name}' (النسخة 1)",
                    false,
                    request.UploadedBy);

                if (timelineResult.IsSuccess)
                    await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(document.Id);
        }
    }
}
