using App.Application.Clients.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Clients.Queries.GetClientById;

public record GetClientByIdQuery(Guid Id) : IQuery<ClientDetailsDto>;
