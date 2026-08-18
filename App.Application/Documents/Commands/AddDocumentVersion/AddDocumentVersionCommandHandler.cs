using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Documents.Entities;
using App.Domain.Documents.Errors;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Commands.AddDocumentVersion
{
    internal sealed class AddDocumentVersionCommandHandler : ICommandHandler<AddDocumentVersionCommand, Guid>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public AddDocumentVersionCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddDocumentVersionCommand request, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(request.DocumentId, cancellationToken);
            if (document is null)
                return Result<Guid>.Failure(DocumentErrors.NotFound(request.DocumentId));

            var existingVersionsCount = await _unitOfWork.DocumentVersions.CountAsync(
                v => v.DocumentId == request.DocumentId, cancellationToken);

            var nextVersionNumber = existingVersionsCount + 1;

            var versionResult = DocumentVersion.Create(
                document.Id,
                nextVersionNumber,
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
            document.SetUpdated(request.UploadedBy);

            if (document.CaseId.HasValue)
            {
                var timelineResult = CaseTimeline.Create(
                    document.CaseId.Value,
                    "تحديث مستند",
                    $"تم رفع نسخة جديدة ({nextVersionNumber}) للمستند '{document.Name}'",
                    false,
                    request.UploadedBy);

                if (timelineResult.IsSuccess)
                    await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(version.Id);
        }
    }
}
