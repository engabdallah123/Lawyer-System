using Shared.Application.Messaging;

namespace App.Application.Consultations.Commands.UpdateConsultation;

public record UpdateConsultationCommand(
    Guid Id,
    DateTime ConsultationDate,
    string Subject,
    string? Description,
    decimal? Fee,
    string? Notes,
    string UpdatedBy) : ICommand;
