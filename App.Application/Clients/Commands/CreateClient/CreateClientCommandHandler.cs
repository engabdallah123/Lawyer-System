using App.Domain;
using App.Domain.Clients.Entities;
using App.Domain.Clients.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Clients.Commands.CreateClient;

internal sealed class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // التحقق من تكرار رقم الهوية إذا وجد
        if (!string.IsNullOrWhiteSpace(request.NationalId))
        {
            var nationalIdExists = await _unitOfWork.Clients.AnyAsync(
                c => c.NationalId == request.NationalId.Trim(), cancellationToken);
            if (nationalIdExists)
                return Result<Guid>.Failure(ClientErrors.DuplicateNationalId);
        }

        // التحقق من تكرار السجل التجاري إذا وجد
        if (!string.IsNullOrWhiteSpace(request.CommercialRegister))
        {
            var crExists = await _unitOfWork.Clients.AnyAsync(
                c => c.CommercialRegister == request.CommercialRegister.Trim(), cancellationToken);
            if (crExists)
                return Result<Guid>.Failure(ClientErrors.DuplicateCommercialRegister);
        }

        var clientResult = Client.Create(
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

        if (clientResult.IsFailure)
            return Result<Guid>.Failure(clientResult.Error);

        var client = clientResult.Value!;
        client.SetCreated(request.CreatedBy);

        await _unitOfWork.Clients.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(client.Id);
    }
}
