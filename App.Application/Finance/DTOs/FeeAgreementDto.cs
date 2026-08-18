using App.Domain.Finance.Enums;

namespace App.Application.Finance.DTOs;

public record FeeAgreementDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    AgreementType AgreementType,
    string AgreementTypeName,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime CreatedAt,
    string CreatedBy);
