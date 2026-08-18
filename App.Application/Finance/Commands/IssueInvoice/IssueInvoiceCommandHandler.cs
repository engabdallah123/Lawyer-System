using App.Domain;
using App.Domain.Finance.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Commands.IssueInvoice;

internal sealed class IssueInvoiceCommandHandler : ICommandHandler<IssueInvoiceCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public IssueInvoiceCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure(FinanceErrors.InvoiceNotFound(request.InvoiceId));

        invoice.Issue();
        invoice.SetUpdated(request.UpdatedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
