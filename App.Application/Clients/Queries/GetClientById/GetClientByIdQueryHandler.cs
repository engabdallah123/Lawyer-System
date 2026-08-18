using App.Application.Clients.DTOs;
using App.Domain.Clients.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Clients.Queries.GetClientById;

internal sealed class GetClientByIdQueryHandler : IQueryHandler<GetClientByIdQuery, ClientDetailsDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetClientByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<ClientDetailsDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                c.Id,
                CASE c.ClientType WHEN 'Individual' THEN 0 WHEN 'Company' THEN 1 ELSE 0 END AS ClientType,
                c.FullName,
                c.CompanyName,
                c.NationalId,
                c.CommercialRegister,
                c.Phone,
                c.Mobile,
                c.Email,
                c.Address,
                c.City,
                c.Notes,
                c.IsActive,
                c.CreatedAt,
                c.CreatedBy,
                c.UpdatedAt,
                c.UpdatedBy,
                (SELECT COUNT(*) FROM CaseParties cp INNER JOIN Cases cs ON cp.CaseId = cs.Id WHERE cp.ClientId = c.Id AND cs.IsDeleted = 0 AND cs.CloseDate IS NULL) AS ActiveCasesCount,
                (SELECT COUNT(*) FROM Consultations cn WHERE cn.ClientId = c.Id) AS TotalConsultationsCount,
                (SELECT COUNT(*) FROM PowerOfAttorneys poa WHERE poa.ClientId = c.Id AND poa.IsActive = 1) AS TotalPowerOfAttorneysCount
            FROM Clients c
            WHERE c.Id = @Id AND c.IsDeleted = 0;";

        var client = await connection.QuerySingleOrDefaultAsync<ClientDetailsDto>(sql, new { Id = request.Id });

        if (client is null)
            return Result<ClientDetailsDto>.Failure(ClientErrors.NotFound(request.Id));

        return Result<ClientDetailsDto>.Success(client);
    }
}
