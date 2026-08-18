using App.Application.Hearings.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Queries.GetHearings;

internal sealed class GetHearingsQueryHandler : IQueryHandler<GetHearingsQuery, IEnumerable<HearingDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetHearingsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<HearingDto>>> Handle(GetHearingsQuery request, CancellationToken cancellationToken)
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
                AND (@FromDate IS NULL OR h.HearingDate >= @FromDate)
                AND (@ToDate IS NULL OR h.HearingDate <= @ToDate)
                AND (@CaseId IS NULL OR h.CaseId = @CaseId)
            ORDER BY h.HearingDate DESC, h.HearingTime ASC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var hearings = await connection.QueryAsync<HearingDto>(
            sql,
            new
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                CaseId = request.CaseId,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<HearingDto>>.Success(hearings);
    }
}
