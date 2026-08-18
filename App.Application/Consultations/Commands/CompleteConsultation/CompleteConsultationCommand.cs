using Shared.Application.Messaging;

namespace App.Application.Consultations.Commands.CompleteConsultation;

public record CompleteConsultationCommand(Guid Id, string UpdatedBy) : ICommand;
