namespace App.Application.Hearings.DTOs;

public record HearingDto(
    Guid Id,
    Guid CaseId,
    string CaseInternalNumber,
    string CaseTitle,
    string? CourtName,
    string? Circuit,
    DateTime HearingDate,
    TimeSpan? HearingTime,
    string HearingType,
    string? Result,
    string? Notes,
    DateTime? NextHearingDate,
    DateTime CreatedAt,
    string CreatedBy);
