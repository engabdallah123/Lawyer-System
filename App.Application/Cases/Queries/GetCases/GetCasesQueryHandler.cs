using App.Application.Cases.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Queries.GetCases;

internal sealed class GetCasesQueryHandler : IQueryHandler<GetCasesQuery, IEnumerable<CaseDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCasesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<CaseDto>>> Handle(GetCasesQuery request, CancellationToken cancellationToken)
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
            INNER JOIN CaseTypes ct ON c.CaseTypeId = ct.Id
            INNER JOIN CaseStatuses cs ON c.CaseStatusId = cs.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.IsDeleted = 0
                AND (@SearchTerm IS NULL OR c.Title LIKE @SearchPattern OR c.InternalNumber LIKE @SearchPattern OR c.CourtNumber LIKE @SearchPattern)
                AND (@CaseTypeId IS NULL OR c.CaseTypeId = @CaseTypeId)
                AND (@CaseStatusId IS NULL OR c.CaseStatusId = @CaseStatusId)
                AND (@CourtId IS NULL OR c.CourtId = @CourtId)
                AND (@IsClosed IS NULL OR (@IsClosed = 1 AND c.CloseDate IS NOT NULL) OR (@IsClosed = 0 AND c.CloseDate IS NULL))
            ORDER BY c.OpenDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;
        var searchPattern = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : $"%{request.SearchTerm.Trim()}%";

        var cases = await connection.QueryAsync<CaseDto>(
            sql,
            new
            {
                SearchTerm = request.SearchTerm,
                SearchPattern = searchPattern,
                CaseTypeId = request.CaseTypeId,
                CaseStatusId = request.CaseStatusId,
                CourtId = request.CourtId,
                IsClosed = request.IsClosed,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<CaseDto>>.Success(cases);
    }
}
