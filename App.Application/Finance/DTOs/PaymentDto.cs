namespace App.Application.Finance.DTOs;

public record PaymentDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    Guid? FeeAgreementId,
    Guid? InvoiceId,
    string? InvoiceNumber,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string? ReferenceNumber,
    string? Notes,
    string ReceivedBy,
    DateTime CreatedAt);
