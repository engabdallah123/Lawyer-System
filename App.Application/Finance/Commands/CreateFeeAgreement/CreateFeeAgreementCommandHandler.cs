using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Clients.Errors;
using App.Domain.Finance.Entities;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Commands.CreateFeeAgreement;

internal sealed class CreateFeeAgreementCommandHandler : ICommandHandler<CreateFeeAgreementCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateFeeAgreementCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateFeeAgreementCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
            return Result<Guid>.Failure(ClientErrors.NotFound(request.ClientId));

        var agreementResult = FeeAgreement.Create(
            request.ClientId,
            request.CaseId,
            request.AgreementType,
            request.TotalAmount,
            request.Description,
            request.StartDate,
            request.EndDate);

        if (agreementResult.IsFailure)
            return Result<Guid>.Failure(agreementResult.Error);

        var agreement = agreementResult.Value!;
        agreement.SetCreated(request.CreatedBy);

        await _unitOfWork.FeeAgreements.AddAsync(agreement, cancellationToken);

        if (request.CaseId.HasValue)
        {
            var timelineResult = CaseTimeline.Create(
                request.CaseId.Value,
                "تسجيل عقد أتعاب",
                $"تم تسجيل عقد أتعاب بمبلغ {request.TotalAmount:N2} د.إ/ر.س",
                true,
                request.CreatedBy);

            if (timelineResult.IsSuccess)
                await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(agreement.Id);
    }
}
