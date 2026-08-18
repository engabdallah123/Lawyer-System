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
    Guid? MainClientId,
    string? MainLawyerUserId,
    string CreatedBy) : ICommand<Guid>;
