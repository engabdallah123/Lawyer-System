namespace App.Application.Cases.DTOs;

public record CaseDto(
    Guid Id,
    string InternalNumber,
    string? CourtNumber,
    string Title,
    int CaseTypeId,
    string CaseTypeName,
    int CaseStatusId,
    string CaseStatusName,
    string? CaseStatusColor,
    int? CourtId,
    string? CourtName,
    string? Circuit,
    string? JudgeName,
    DateTime OpenDate,
    DateTime? CloseDate,
    decimal? ClaimAmount,
    string? CurrentStage,
    DateTime CreatedAt);
