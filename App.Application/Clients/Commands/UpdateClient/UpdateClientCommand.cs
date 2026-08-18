using App.Domain.Clients.Enums;
using Shared.Application.Messaging;

namespace App.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id,
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
    string UpdatedBy) : ICommand;
