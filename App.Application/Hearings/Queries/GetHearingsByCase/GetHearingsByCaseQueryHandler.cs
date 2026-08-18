using App.Application.Hearings.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Queries.GetHearingsByCase;

internal sealed class GetHearingsByCaseQueryHandler : IQueryHandler<GetHearingsByCaseQuery, IEnumerable<HearingDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetHearingsByCaseQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<HearingDto>>> Handle(GetHearingsByCaseQuery request, CancellationToken cancellationToken)
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
            WHERE h.CaseId = @CaseId AND c.IsDeleted = 0
            ORDER BY h.HearingDate DESC, h.HearingTime ASC;";

        var hearings = await connection.QueryAsync<HearingDto>(sql, new { CaseId = request.CaseId });
        return Result<IEnumerable<HearingDto>>.Success(hearings);
    }
}
