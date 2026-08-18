using App.Application.Clients.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Clients.Queries.SearchClients;

public record SearchClientsQuery(string Query, int MaxResults = 10) : IQuery<IEnumerable<ClientDto>>;
