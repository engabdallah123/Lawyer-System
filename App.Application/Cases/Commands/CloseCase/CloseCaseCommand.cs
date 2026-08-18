using Shared.Application.Messaging;

namespace App.Application.Cases.Commands.CloseCase;

public record CloseCaseCommand(Guid CaseId, DateTime CloseDate, string ClosedBy) : ICommand;
