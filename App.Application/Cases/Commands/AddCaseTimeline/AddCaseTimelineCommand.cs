using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.AddCaseTimeline;

public record AddCaseTimelineCommand(
    Guid CaseId,
    string Title,
    string? Description,
    bool IsImportant,
    string CreatedBy) : ICommand<Guid>;
