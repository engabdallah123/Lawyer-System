using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Clients.Errors;
using App.Domain.Finance.Entities;
using App.Domain.Finance.Enums;
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

        // 1. Direct Fee Agreement Allocation
        if (request.FeeAgreementId.HasValue)
        {
            var feeAgreement = await _unitOfWork.FeeAgreements.GetByIdAsync(request.FeeAgreementId.Value, cancellationToken);
            if (feeAgreement != null)
            {
                feeAgreement.AddPaymentAmount(request.Amount);
                feeAgreement.SetUpdated(request.ReceivedBy);
            }
        }
        // Auto-allocate to Case Fee Agreements if not specified
        else if (request.CaseId.HasValue)
        {
            var openAgreements = await _unitOfWork.FeeAgreements.FindAllAsync(
                fa => fa.CaseId == request.CaseId.Value && fa.PaidAmount < fa.TotalAmount,
                [],
                cancellationToken);

            var remainingToDistribute = request.Amount;
            foreach (var fa in openAgreements.OrderBy(a => a.StartDate))
            {
                if (remainingToDistribute <= 0) break;
                var due = fa.TotalAmount - fa.PaidAmount;
                var paymentPart = Math.Min(due, remainingToDistribute);
                fa.AddPaymentAmount(paymentPart);
                fa.SetUpdated(request.ReceivedBy);
                remainingToDistribute -= paymentPart;
            }
        }

        // 2. Direct Invoice Allocation
        if (request.InvoiceId.HasValue)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId.Value, cancellationToken);
            if (invoice != null)
            {
                invoice.AddPaymentAmount(request.Amount);
                invoice.SetUpdated(request.ReceivedBy);
            }
        }
        // Auto-allocate to Case Invoices if not specified
        else if (request.CaseId.HasValue)
        {
            var openInvoices = await _unitOfWork.Invoices.FindAllAsync(
                i => i.CaseId == request.CaseId.Value && i.PaidAmount < i.TotalAmount && i.Status != InvoiceStatus.Cancelled,
                [],
                cancellationToken);

            var remainingToDistribute = request.Amount;
            foreach (var inv in openInvoices.OrderBy(i => i.IssueDate))
            {
                if (remainingToDistribute <= 0) break;
                var due = inv.TotalAmount - inv.PaidAmount;
                var paymentPart = Math.Min(due, remainingToDistribute);
                inv.AddPaymentAmount(paymentPart);
                inv.SetUpdated(request.ReceivedBy);
                remainingToDistribute -= paymentPart;
            }
        }
        // Auto-allocate to Client Invoices if neither Case nor Invoice specified
        else
        {
            var openInvoices = await _unitOfWork.Invoices.FindAllAsync(
                i => i.ClientId == request.ClientId && i.PaidAmount < i.TotalAmount && i.Status != InvoiceStatus.Cancelled,
                [],
                cancellationToken);

            var remainingToDistribute = request.Amount;
            foreach (var inv in openInvoices.OrderBy(i => i.IssueDate))
            {
                if (remainingToDistribute <= 0) break;
                var due = inv.TotalAmount - inv.PaidAmount;
                var paymentPart = Math.Min(due, remainingToDistribute);
                inv.AddPaymentAmount(paymentPart);
                inv.SetUpdated(request.ReceivedBy);
                remainingToDistribute -= paymentPart;
            }
        }

        // 3. Timeline Entry
        if (request.CaseId.HasValue)
        {
            var timelineResult = CaseTimeline.Create(
                request.CaseId.Value,
                "تسجيل سند قبض وتحصيل",
                $"تم استلام وتحصيل مبلغ {request.Amount:N2} ج.م عبر {request.PaymentMethod}",
                false,
                request.ReceivedBy);

            if (timelineResult.IsSuccess)
                await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(payment.Id);
    }
}
