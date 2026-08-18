using App.Application.PowerOfAttorney.DTOs;
using App.Domain.PowerOfAttorney.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.PowerOfAttorney.Queries.GetPowerOfAttorneyById;

internal sealed class GetPowerOfAttorneyByIdQueryHandler : IQueryHandler<GetPowerOfAttorneyByIdQuery, PowerOfAttorneyDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPowerOfAttorneyByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PowerOfAttorneyDto>> Handle(GetPowerOfAttorneyByIdQuery request, CancellationToken cancellationToken)
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
            WHERE p.Id = @Id;";

        var poa = await connection.QuerySingleOrDefaultAsync<PowerOfAttorneyDto>(sql, new { Id = request.Id });
        if (poa is null)
            return Result<PowerOfAttorneyDto>.Failure(PowerOfAttorneyErrors.NotFound(request.Id));

        return Result<PowerOfAttorneyDto>.Success(poa);
    }
}
