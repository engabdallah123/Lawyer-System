using App.Application.Hearings.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Hearings.Queries.GetMonthlyCalendarHearings;

public record GetMonthlyCalendarHearingsQuery(int Year, int Month, string? LawyerUserId = null) : IQuery<IEnumerable<CalendarHearingDaySummaryDto>>;
