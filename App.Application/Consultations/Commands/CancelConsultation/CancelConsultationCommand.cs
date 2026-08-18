using Shared.Application.Messaging;

namespace App.Application.Consultations.Commands.CancelConsultation;

public record CancelConsultationCommand(Guid Id, string UpdatedBy) : ICommand;
