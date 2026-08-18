using App.Domain;
using App.Domain.Tasks.Errors;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.UpdateLegalTask
{
    internal sealed class UpdateLegalTaskCommandHandler : ICommandHandler<UpdateLegalTaskCommand>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public UpdateLegalTaskCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateLegalTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            if (task is null)
                return Result.Failure(TaskErrors.NotFound(request.Id));

            var updateResult = task.Update(
                request.Title,
                request.Description,
                request.DueDate,
                request.Priority);

            if (updateResult.IsFailure)
                return updateResult;

            task.SetUpdated(request.UpdatedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
