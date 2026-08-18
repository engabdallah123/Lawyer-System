using App.Domain;
using App.Domain.Clients.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.PowerOfAttorney.Commands.CreatePowerOfAttorney;

internal sealed class CreatePowerOfAttorneyCommandHandler : ICommandHandler<CreatePowerOfAttorneyCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreatePowerOfAttorneyCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePowerOfAttorneyCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
            return Result<Guid>.Failure(ClientErrors.NotFound(request.ClientId));

        var poaResult = Domain.PowerOfAttorney.Entities.PowerOfAttorney.Create(
            request.ClientId,
            request.CaseId,
            request.PowerNumber,
            request.IssueDate,
            request.ExpiryDate,
            request.NotaryName,
            request.NotaryNumber,
            request.FilePath,
            request.Notes);

        if (poaResult.IsFailure)
            return Result<Guid>.Failure(poaResult.Error);

        var poa = poaResult.Value!;
        poa.SetCreated(request.CreatedBy);

        await _unitOfWork.PowerOfAttorneys.AddAsync(poa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(poa.Id);
    }
}
