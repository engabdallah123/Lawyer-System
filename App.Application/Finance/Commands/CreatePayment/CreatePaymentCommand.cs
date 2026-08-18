using Shared.Application.Messaging;

namespace App.Application.Finance.Commands.CreatePayment;

public record CreatePaymentCommand(
    Guid ClientId,
    Guid? CaseId,
    Guid? FeeAgreementId,
    Guid? InvoiceId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string? ReferenceNumber,
    string? Notes,
    string ReceivedBy) : ICommand<Guid>;
