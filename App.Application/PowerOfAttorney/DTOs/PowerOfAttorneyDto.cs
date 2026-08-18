namespace App.Application.PowerOfAttorney.DTOs;

public record PowerOfAttorneyDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string? ClientPhone,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    string PowerNumber,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? NotaryName,
    string? NotaryNumber,
    string? FilePath,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy);
