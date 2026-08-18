using App.Domain;
using App.Domain.Cases.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.UpdateCase;

internal sealed class UpdateCaseCommandHandler : ICommandHandler<UpdateCaseCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public UpdateCaseCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _unitOfWork.Cases.GetByIdAsync(request.Id, cancellationToken);
        if (caseEntity is null)
            return Result.Failure(CaseErrors.NotFound(request.Id));

        var updateResult = caseEntity.Update(
            request.InternalNumber,
            request.CourtNumber,
            request.Title,
            request.CaseTypeId,
            request.CaseStatusId,
            request.CourtId,
            request.Circuit,
            request.JudgeName,
            request.ClaimAmount,
            request.Description,
            request.CurrentStage,
            request.Notes);

        if (updateResult.IsFailure)
            return updateResult;

        caseEntity.SetUpdated(request.UpdatedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
