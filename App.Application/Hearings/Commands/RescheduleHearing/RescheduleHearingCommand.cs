using Shared.Application.Messaging;

namespace App.Application.Hearings.Commands.RescheduleHearing;

public record RescheduleHearingCommand(
    Guid HearingId,
    DateTime NewDate,
    TimeSpan? NewTime,
    string RescheduledBy) : ICommand;
