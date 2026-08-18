using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.UpdateCase;

public record UpdateCaseCommand(
    Guid Id,
    string InternalNumber,
    string? CourtNumber,
    string Title,
    int CaseTypeId,
    int CaseStatusId,
    int? CourtId,
    string? Circuit,
    string? JudgeName,
    decimal? ClaimAmount,
    string? Description,
    string? CurrentStage,
    string? Notes,
    string UpdatedBy) : ICommand;
