using App.Application.Hearings.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Queries.GetMonthlyCalendarHearings;

internal sealed class GetMonthlyCalendarHearingsQueryHandler : IQueryHandler<GetMonthlyCalendarHearingsQuery, IEnumerable<CalendarHearingDaySummaryDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetMonthlyCalendarHearingsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<CalendarHearingDaySummaryDto>>> Handle(GetMonthlyCalendarHearingsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var sql = @"
            SELECT 
                h.Id,
                h.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                crt.Name AS CourtName,
                h.HearingDate,
                h.HearingTime,
                h.HearingType,
                h.Result
            FROM Hearings h
            INNER JOIN Cases c ON h.CaseId = c.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.IsDeleted = 0
                AND h.HearingDate >= @StartDate AND h.HearingDate <= @EndDate
                AND (@LawyerUserId IS NULL OR EXISTS (
                    SELECT 1 FROM CaseAssignments ca WHERE ca.CaseId = c.Id AND ca.UserId = @LawyerUserId
                ))
            ORDER BY h.HearingDate ASC, h.HearingTime ASC;";

        var flatHearings = (await connection.QueryAsync<dynamic>(
            sql,
            new { StartDate = startDate, EndDate = endDate, LawyerUserId = request.LawyerUserId })).ToList();

        var grouped = flatHearings
            .GroupBy(h => ((DateTime)h.HearingDate).Date)
            .Select(g => new CalendarHearingDaySummaryDto(
                g.Key,
                g.Count(),
                g.Select(item => new CalendarHearingItemDto(
                    (Guid)item.Id,
                    (Guid)item.CaseId,
                    (string)item.CaseInternalNumber,
                    (string)item.CaseTitle,
                    (string)item.CourtName,
                    (TimeSpan?)item.HearingTime,
                    (string)item.HearingType,
                    (string)item.Result
                ))
            ))
            .ToList();

        return Result<IEnumerable<CalendarHearingDaySummaryDto>>.Success(grouped);
    }
}
