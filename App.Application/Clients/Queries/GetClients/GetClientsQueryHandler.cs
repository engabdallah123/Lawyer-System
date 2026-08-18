using App.Application.Clients.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Clients.Queries.GetClients;

internal sealed class GetClientsQueryHandler : IQueryHandler<GetClientsQuery, IEnumerable<ClientDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetClientsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<ClientDto>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                Id,
                CASE ClientType WHEN 'Individual' THEN 0 WHEN 'Company' THEN 1 ELSE 0 END AS ClientType,
                FullName,
                CompanyName,
                NationalId,
                CommercialRegister,
                Phone,
                Mobile,
                Email,
                Address,
                City,
                Notes,
                IsActive,
                CreatedAt
            FROM Clients
            WHERE IsDeleted = 0
                AND (@SearchTerm IS NULL OR FullName LIKE @SearchPattern OR CompanyName LIKE @SearchPattern OR Phone LIKE @SearchPattern OR NationalId LIKE @SearchPattern)
                AND (@ClientType IS NULL OR ClientType = @ClientTypeName)
                AND (@IsActive IS NULL OR IsActive = @IsActive)
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var clientTypeName = request.ClientType switch
        {
            0 => "Individual",
            1 => "Company",
            _ => null
        };

        var offset = (request.Page - 1) * request.PageSize;
        var searchPattern = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : $"%{request.SearchTerm.Trim()}%";

        var clients = await connection.QueryAsync<ClientDto>(
            sql,
            new
            {
                SearchTerm = request.SearchTerm,
                SearchPattern = searchPattern,
                ClientType = request.ClientType,
                ClientTypeName = clientTypeName,
                IsActive = request.IsActive,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<ClientDto>>.Success(clients);
    }
}
