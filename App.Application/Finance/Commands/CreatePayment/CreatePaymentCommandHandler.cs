using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Clients.Errors;
using App.Domain.Finance.Entities;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Commands.CreatePayment;

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
            return Result<Guid>.Failure(ClientErrors.NotFound(request.ClientId));

        var paymentResult = Payment.Create(
            request.ClientId,
            request.CaseId,
            request.FeeAgreementId,
            request.InvoiceId,
            request.Amount,
            request.PaymentDate,
            request.PaymentMethod,
            request.ReferenceNumber,
            request.Notes,
            request.ReceivedBy);

        if (paymentResult.IsFailure)
            return Result<Guid>.Failure(paymentResult.Error);

        var payment = paymentResult.Value!;
        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

        if (request.FeeAgreementId.HasValue)
        {
            var feeAgreement = await _unitOfWork.FeeAgreements.GetByIdAsync(request.FeeAgreementId.Value, cancellationToken);
            if (feeAgreement != null)
            {
                feeAgreement.AddPaymentAmount(request.Amount);
                feeAgreement.SetUpdated(request.ReceivedBy);
            }
        }

        if (request.InvoiceId.HasValue)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId.Value, cancellationToken);
            if (invoice != null)
            {
                invoice.AddPaymentAmount(request.Amount);
                invoice.SetUpdated(request.ReceivedBy);
            }
        }

        if (request.CaseId.HasValue)
        {
            var timelineResult = CaseTimeline.Create(
                request.CaseId.Value,
                "تسجيل دفعة مالية",
                $"تم استلام دفعة مالية بقيمة {request.Amount:N2} عبر {request.PaymentMethod}",
                false,
                request.ReceivedBy);

            if (timelineResult.IsSuccess)
                await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(payment.Id);
    }
}
