namespace App.Application.Hearings.DTOs;

public record CalendarHearingItemDto(
    Guid Id,
    Guid CaseId,
    string CaseInternalNumber,
    string CaseTitle,
    string? CourtName,
    TimeSpan? HearingTime,
    string HearingType,
    string? Result);
