using App.Application.Hearings.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Queries.GetTodayHearings;

internal sealed class GetTodayHearingsQueryHandler : IQueryHandler<GetTodayHearingsQuery, IEnumerable<HearingDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetTodayHearingsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<HearingDto>>> Handle(GetTodayHearingsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                h.Id,
                h.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                crt.Name AS CourtName,
                c.Circuit,
                h.HearingDate,
                h.HearingTime,
                h.HearingType,
                h.Result,
                h.Notes,
                h.NextHearingDate,
                h.CreatedAt,
                h.CreatedBy
            FROM Hearings h
            INNER JOIN Cases c ON h.CaseId = c.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.IsDeleted = 0
                AND CAST(h.HearingDate AS DATE) = CAST(GETUTCDATE() AS DATE)
                AND (@LawyerUserId IS NULL OR EXISTS (
                    SELECT 1 FROM CaseAssignments ca WHERE ca.CaseId = c.Id AND ca.UserId = @LawyerUserId
                ))
            ORDER BY h.HearingTime ASC;";

        var hearings = await connection.QueryAsync<HearingDto>(sql, new { LawyerUserId = request.LawyerUserId });
        return Result<IEnumerable<HearingDto>>.Success(hearings);
    }
}
