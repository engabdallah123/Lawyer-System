using Shared.Application.Messaging;

namespace App.Application.Clients.Commands.DeleteClient;

public record DeleteClientCommand(Guid Id, string DeletedBy) : ICommand;
