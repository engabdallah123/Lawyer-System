using App.Domain.Cases.Enums;
using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.AddCaseParty;

public record AddCasePartyCommand(
    Guid CaseId,
    Guid? ClientId,
    string? PartyName,
    PartyRole PartyRole,
    bool IsMainClient,
    string? Notes) : ICommand<Guid>;
