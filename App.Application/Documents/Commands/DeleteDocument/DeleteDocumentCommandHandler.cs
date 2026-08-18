using App.Domain;
using App.Domain.Documents.Errors;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Commands.DeleteDocument
{
    internal sealed class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public DeleteDocumentCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(request.Id, cancellationToken);
            if (document is null)
                return Result.Failure(DocumentErrors.NotFound(request.Id));

            document.SoftDelete(request.DeletedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
