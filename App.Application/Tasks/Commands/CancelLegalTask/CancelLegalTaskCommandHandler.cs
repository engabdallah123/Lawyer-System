using App.Domain;
using App.Domain.Tasks.Errors;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.CancelLegalTask
{
    internal sealed class CancelLegalTaskCommandHandler : ICommandHandler<CancelLegalTaskCommand>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public CancelLegalTaskCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelLegalTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            if (task is null)
                return Result.Failure(TaskErrors.NotFound(request.Id));

            task.Cancel();
            task.SetUpdated(request.UpdatedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
