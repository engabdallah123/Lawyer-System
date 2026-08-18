namespace App.Application.Hearings.DTOs;

public record CalendarHearingDaySummaryDto(
    DateTime Date,
    int HearingsCount,
    IEnumerable<CalendarHearingItemDto> Hearings);
