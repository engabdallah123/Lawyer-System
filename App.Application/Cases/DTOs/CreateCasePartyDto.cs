using App.Domain.Cases.Enums;

namespace App.Application.Cases.DTOs;

public record CreateCasePartyDto(
    Guid? ClientId = null,
    string? PartyName = null,
    string? PartyType = "شخص / فرد",
    PartyRole PartyRole = PartyRole.Defendant,
    bool IsMainClient = false,
    string? Address = null,
    string? Phone = null,
    string? Email = null,
    string? LawyerName = null,
    string? LawyerPhone = null,
    string? Notes = null);
