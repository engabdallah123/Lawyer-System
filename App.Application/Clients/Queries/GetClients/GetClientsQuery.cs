using App.Application.Clients.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Clients.Queries.GetClients;

public record GetClientsQuery(
    string? SearchTerm = null,
    int? ClientType = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<ClientDto>>;
