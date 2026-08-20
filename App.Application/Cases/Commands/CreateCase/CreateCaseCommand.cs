using App.Application.Cases.DTOs;
using App.Domain.Cases.Enums;
using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(
    string InternalNumber,
    string? CourtNumber,
    string Title,
    int CaseTypeId,
    int CaseStatusId,
    int? CourtId,
    string? Circuit,
    string? JudgeName,
    DateTime OpenDate,
    decimal? ClaimAmount,
    string? Description,
    string? CurrentStage,
    string? Notes,
    Guid ClientId,
    PartyRole ClientRole,
    List<CreateCasePartyDto>? AdditionalParties,
    string? MainLawyerUserId,
    string CreatedBy) : ICommand<Guid>;
