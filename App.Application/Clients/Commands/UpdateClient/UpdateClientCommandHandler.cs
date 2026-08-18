using App.Domain;
using App.Domain.Clients.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Clients.Commands.UpdateClient;

internal sealed class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public UpdateClientCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
            return Result.Failure(ClientErrors.NotFound(request.Id));

        if (!string.IsNullOrWhiteSpace(request.NationalId))
        {
            var duplicate = await _unitOfWork.Clients.AnyAsync(
                c => c.NationalId == request.NationalId.Trim() && c.Id != request.Id, cancellationToken);
            if (duplicate)
                return Result.Failure(ClientErrors.DuplicateNationalId);
        }

        if (!string.IsNullOrWhiteSpace(request.CommercialRegister))
        {
            var duplicate = await _unitOfWork.Clients.AnyAsync(
                c => c.CommercialRegister == request.CommercialRegister.Trim() && c.Id != request.Id, cancellationToken);
            if (duplicate)
                return Result.Failure(ClientErrors.DuplicateCommercialRegister);
        }

        var updateResult = client.Update(
            request.ClientType,
            request.FullName,
            request.CompanyName,
            request.NationalId,
            request.CommercialRegister,
            request.Phone,
            request.Mobile,
            request.Email,
            request.Address,
            request.City,
            request.Notes);

        if (updateResult.IsFailure)
            return updateResult;

        client.SetUpdated(request.UpdatedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
