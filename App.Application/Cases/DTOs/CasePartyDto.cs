using App.Domain.Cases.Enums;

namespace App.Application.Cases.DTOs;

public record CasePartyDto(
    Guid Id,
    Guid CaseId,
    Guid? ClientId,
    string? ClientName,
    string? PartyName,
    PartyRole PartyRole,
    string PartyRoleName,
    bool IsMainClient,
    string? Notes);
