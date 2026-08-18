using App.Application.PowerOfAttorney.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.PowerOfAttorney.Queries.GetExpiringPowerOfAttorneys;

internal sealed class GetExpiringPowerOfAttorneysQueryHandler : IQueryHandler<GetExpiringPowerOfAttorneysQuery, IEnumerable<PowerOfAttorneyDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetExpiringPowerOfAttorneysQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<PowerOfAttorneyDto>>> Handle(GetExpiringPowerOfAttorneysQuery request, CancellationToken cancellationToken)
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
            WHERE p.IsActive = 1
                AND p.ExpiryDate IS NOT NULL
                AND p.ExpiryDate <= DATEADD(day, @WithinDays, GETUTCDATE())
                AND p.ExpiryDate >= GETUTCDATE()
            ORDER BY p.ExpiryDate ASC;";

        var poas = await connection.QueryAsync<PowerOfAttorneyDto>(sql, new { WithinDays = request.WithinDays });
        return Result<IEnumerable<PowerOfAttorneyDto>>.Success(poas);
    }
}
