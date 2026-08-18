using Shared.Application.Messaging;

namespace App.Application.Finance.Commands.CreateInvoice;

public record CreateInvoiceItemRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice);

public record CreateInvoiceCommand(
    string InvoiceNumber,
    Guid ClientId,
    Guid? CaseId,
    Guid? FeeAgreementId,
    DateTime IssueDate,
    DateTime? DueDate,
    decimal Discount,
    decimal TaxAmount,
    string? Notes,
    List<CreateInvoiceItemRequest> Items,
    string CreatedBy) : ICommand<Guid>;
