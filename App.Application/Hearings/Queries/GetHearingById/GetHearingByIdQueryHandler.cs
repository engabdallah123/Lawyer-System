using App.Application.Hearings.DTOs;
using App.Domain.Hearings.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Queries.GetHearingById;

internal sealed class GetHearingByIdQueryHandler : IQueryHandler<GetHearingByIdQuery, HearingDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetHearingByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<HearingDto>> Handle(GetHearingByIdQuery request, CancellationToken cancellationToken)
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
            WHERE h.Id = @Id AND c.IsDeleted = 0;";

        var hearing = await connection.QuerySingleOrDefaultAsync<HearingDto>(sql, new { Id = request.Id });
        if (hearing is null)
            return Result<HearingDto>.Failure(HearingErrors.NotFound(request.Id));

        return Result<HearingDto>.Success(hearing);
    }
}
