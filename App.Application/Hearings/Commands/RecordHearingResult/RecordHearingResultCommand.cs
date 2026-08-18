using Shared.Application.Messaging;

namespace App.Application.Hearings.Commands.RecordHearingResult;

public record RecordHearingResultCommand(
    Guid HearingId,
    string? Result,
    string? Notes,
    DateTime? NextHearingDate,
    TimeSpan? NextHearingTime,
    string? NextHearingType,
    string UpdatedBy) : ICommand;
