using App.Application.PowerOfAttorney.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.PowerOfAttorney.Queries.GetPowerOfAttorneys;

internal sealed class GetPowerOfAttorneysQueryHandler : IQueryHandler<GetPowerOfAttorneysQuery, IEnumerable<PowerOfAttorneyDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPowerOfAttorneysQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<PowerOfAttorneyDto>>> Handle(GetPowerOfAttorneysQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                p.Id,
                p.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                cl.Phone AS ClientPhone,
                p.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                p.PowerNumber,
                p.IssueDate,
                p.ExpiryDate,
                p.NotaryName,
                p.NotaryNumber,
                p.FilePath,
                p.Notes,
                p.IsActive,
                p.CreatedAt,
                p.CreatedBy
            FROM PowerOfAttorneys p
            INNER JOIN Clients cl ON p.ClientId = cl.Id
            LEFT JOIN Cases c ON p.CaseId = c.Id
            WHERE (@ClientId IS NULL OR p.ClientId = @ClientId)
                AND (@CaseId IS NULL OR p.CaseId = @CaseId)
                AND (@IsActive IS NULL OR p.IsActive = @IsActive)
            ORDER BY p.IssueDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var poas = await connection.QueryAsync<PowerOfAttorneyDto>(
            sql,
            new
            {
                ClientId = request.ClientId,
                CaseId = request.CaseId,
                IsActive = request.IsActive,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<PowerOfAttorneyDto>>.Success(poas);
    }
}
