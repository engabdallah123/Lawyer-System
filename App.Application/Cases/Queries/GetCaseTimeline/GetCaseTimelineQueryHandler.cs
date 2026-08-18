using App.Application.Cases.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Queries.GetCaseTimeline;

internal sealed class GetCaseTimelineQueryHandler : IQueryHandler<GetCaseTimelineQuery, IEnumerable<CaseTimelineDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCaseTimelineQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<CaseTimelineDto>>> Handle(GetCaseTimelineQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                Id,
                CaseId,
                Title,
                Description,
                IsImportant,
                CreatedAt,
                CreatedBy
            FROM CaseTimelines
            WHERE CaseId = @CaseId
            ORDER BY CreatedAt DESC;";

        var timelines = await connection.QueryAsync<CaseTimelineDto>(sql, new { CaseId = request.CaseId });
        return Result<IEnumerable<CaseTimelineDto>>.Success(timelines);
    }
}
