using Shared.Application.Messaging;

namespace App.Application.Hearings.Commands.CreateHearing;

public record CreateHearingCommand(
    Guid CaseId,
    DateTime HearingDate,
    TimeSpan? HearingTime,
    string HearingType,
    string? Notes,
    string CreatedBy) : ICommand<Guid>;
