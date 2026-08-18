using App.Domain;
using App.Domain.PowerOfAttorney.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.PowerOfAttorney.Commands.UpdatePowerOfAttorney;

internal sealed class UpdatePowerOfAttorneyCommandHandler : ICommandHandler<UpdatePowerOfAttorneyCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public UpdatePowerOfAttorneyCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePowerOfAttorneyCommand request, CancellationToken cancellationToken)
    {
        var poa = await _unitOfWork.PowerOfAttorneys.GetByIdAsync(request.Id, cancellationToken);
        if (poa is null)
            return Result.Failure(PowerOfAttorneyErrors.NotFound(request.Id));

        var updateResult = poa.Update(
            request.CaseId,
            request.PowerNumber,
            request.IssueDate,
            request.ExpiryDate,
            request.NotaryName,
            request.NotaryNumber,
            request.Notes);

        if (updateResult.IsFailure)
            return updateResult;

        poa.SetUpdated(request.UpdatedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
