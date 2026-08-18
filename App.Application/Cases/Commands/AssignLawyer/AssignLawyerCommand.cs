using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.AssignLawyer;

public record AssignLawyerCommand(
    Guid CaseId,
    string UserId,
    string RoleInCase,
    DateTime AssignedDate,
    string? Notes,
    string AssignedBy) : ICommand<Guid>;
