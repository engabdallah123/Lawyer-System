using App.Application.Cases.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Queries.GetCasesByLawyer;

internal sealed class GetCasesByLawyerQueryHandler : IQueryHandler<GetCasesByLawyerQuery, IEnumerable<CaseDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCasesByLawyerQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<CaseDto>>> Handle(GetCasesByLawyerQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                c.Id,
                c.InternalNumber,
                c.CourtNumber,
                c.Title,
                c.CaseTypeId,
                ct.Name AS CaseTypeName,
                c.CaseStatusId,
                cs.Name AS CaseStatusName,
                cs.Color AS CaseStatusColor,
                c.CourtId,
                crt.Name AS CourtName,
                c.Circuit,
                c.JudgeName,
                c.OpenDate,
                c.CloseDate,
                c.ClaimAmount,
                c.CurrentStage,
                c.CreatedAt
            FROM Cases c
            INNER JOIN CaseAssignments ca ON c.Id = ca.CaseId
            INNER JOIN CaseTypes ct ON c.CaseTypeId = ct.Id
            INNER JOIN CaseStatuses cs ON c.CaseStatusId = cs.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.IsDeleted = 0
                AND ca.UserId = @UserId
                AND (@IsClosed IS NULL OR (@IsClosed = 1 AND c.CloseDate IS NOT NULL) OR (@IsClosed = 0 AND c.CloseDate IS NULL))
            ORDER BY c.OpenDate DESC;";

        var cases = await connection.QueryAsync<CaseDto>(
            sql,
            new { UserId = request.UserId, IsClosed = request.IsClosed });

        return Result<IEnumerable<CaseDto>>.Success(cases);
    }
}
