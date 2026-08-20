using App.Domain.Cases.Enums;
using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.AddCaseParty;

public record AddCasePartyCommand(
    Guid CaseId,
    Guid? ClientId,
    string? PartyName,
    string? PartyType,
    PartyRole PartyRole,
    bool IsMainClient,
    string? Address = null,
    string? Phone = null,
    string? Email = null,
    string? LawyerName = null,
    string? LawyerPhone = null,
    string? Notes = null) : ICommand<Guid>;
