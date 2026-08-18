using App.Domain.Clients.Enums;
using Shared.Application.Messaging;

namespace App.Application.Clients.Commands.CreateClient;

public record CreateClientCommand(
    ClientType ClientType,
    string? FullName,
    string? CompanyName,
    string? NationalId,
    string? CommercialRegister,
    string Phone,
    string? Mobile,
    string? Email,
    string? Address,
    string? City,
    string? Notes,
    string CreatedBy) : ICommand<Guid>;
