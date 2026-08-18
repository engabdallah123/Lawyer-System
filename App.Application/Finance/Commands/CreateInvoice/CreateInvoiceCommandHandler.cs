using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Clients.Errors;
using App.Domain.Finance.Entities;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Commands.CreateInvoice;

internal sealed class CreateInvoiceCommandHandler : ICommandHandler<CreateInvoiceCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateInvoiceCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
            return Result<Guid>.Failure(ClientErrors.NotFound(request.ClientId));

        decimal subTotal = 0;
        foreach (var item in request.Items)
        {
            subTotal += (item.Quantity * item.UnitPrice);
        }

        var invoiceResult = Invoice.Create(
            request.InvoiceNumber,
            request.ClientId,
            request.CaseId,
            request.FeeAgreementId,
            request.IssueDate,
            request.DueDate,
            subTotal,
            request.Discount,
            request.TaxAmount,
            request.Notes);

        if (invoiceResult.IsFailure)
            return Result<Guid>.Failure(invoiceResult.Error);

        var invoice = invoiceResult.Value!;
        invoice.SetCreated(request.CreatedBy);

        await _unitOfWork.Invoices.AddAsync(invoice, cancellationToken);

        foreach (var itemReq in request.Items)
        {
            var itemResult = InvoiceItem.Create(invoice.Id, itemReq.Description, itemReq.Quantity, itemReq.UnitPrice);
            if (itemResult.IsSuccess)
            {
                await _unitOfWork.InvoiceItems.AddAsync(itemResult.Value!, cancellationToken);
            }
        }

        if (request.CaseId.HasValue)
        {
            var timelineResult = CaseTimeline.Create(
                request.CaseId.Value,
                "إصدار فاتورة",
                $"تم إنشاء فاتورة برقم '{request.InvoiceNumber}' بإجمالي {invoice.TotalAmount:N2}",
                false,
                request.CreatedBy);

            if (timelineResult.IsSuccess)
                await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(invoice.Id);
    }
}
