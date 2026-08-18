using App.Domain.Finance.Enums;

namespace App.Application.Finance.DTOs;

public record InvoiceDetailsDto(
    Guid Id,
    string InvoiceNumber,
    Guid ClientId,
    string ClientName,
    string? ClientPhone,
    string? ClientAddress,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    Guid? FeeAgreementId,
    DateTime IssueDate,
    DateTime? DueDate,
    decimal SubTotal,
    decimal Discount,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    InvoiceStatus Status,
    string StatusName,
    string? Notes,
    string? QRCodePath,
    DateTime CreatedAt,
    string CreatedBy,
    IEnumerable<InvoiceItemDto> Items,
    IEnumerable<PaymentDto> Payments);
