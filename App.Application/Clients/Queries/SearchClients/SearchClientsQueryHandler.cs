using App.Application.Clients.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Clients.Queries.SearchClients;

internal sealed class SearchClientsQueryHandler : IQueryHandler<SearchClientsQuery, IEnumerable<ClientDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public SearchClientsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<ClientDto>>> Handle(SearchClientsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return Result<IEnumerable<ClientDto>>.Success(Enumerable.Empty<ClientDto>());

        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT TOP (@MaxResults)
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
                AND (FullName LIKE @SearchPattern OR CompanyName LIKE @SearchPattern OR Phone LIKE @SearchPattern OR NationalId LIKE @SearchPattern)
            ORDER BY FullName, CompanyName;";

        var clients = await connection.QueryAsync<ClientDto>(
            sql,
            new
            {
                MaxResults = request.MaxResults,
                SearchPattern = $"%{request.Query.Trim()}%"
            });

        return Result<IEnumerable<ClientDto>>.Success(clients);
    }
}
