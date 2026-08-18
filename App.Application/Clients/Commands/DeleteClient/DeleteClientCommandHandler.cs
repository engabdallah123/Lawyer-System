using App.Domain;
using App.Domain.Clients.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Clients.Commands.DeleteClient;

internal sealed class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public DeleteClientCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
            return Result.Failure(ClientErrors.NotFound(request.Id));

        var hasCases = await _unitOfWork.CaseParties.AnyAsync(
            cp => cp.ClientId == request.Id, cancellationToken);
        if (hasCases)
            return Result.Failure(ClientErrors.HasRelatedData);

        client.SoftDelete(request.DeletedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
