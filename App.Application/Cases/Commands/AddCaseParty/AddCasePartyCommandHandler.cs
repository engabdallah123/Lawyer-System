using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.AddCaseParty;

internal sealed class AddCasePartyCommandHandler : ICommandHandler<AddCasePartyCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public AddCasePartyCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddCasePartyCommand request, CancellationToken cancellationToken)
    {
        var caseExists = await _unitOfWork.Cases.AnyAsync(c => c.Id == request.CaseId, cancellationToken);
        if (!caseExists)
            return Result<Guid>.Failure(CaseErrors.NotFound(request.CaseId));

        var partyResult = CaseParty.Create(
            request.CaseId,
            request.ClientId,
            request.PartyName,
            request.PartyRole,
            request.IsMainClient,
            request.Notes);

        if (partyResult.IsFailure)
            return Result<Guid>.Failure(partyResult.Error);

        await _unitOfWork.CaseParties.AddAsync(partyResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(partyResult.Value!.Id);
    }
}
