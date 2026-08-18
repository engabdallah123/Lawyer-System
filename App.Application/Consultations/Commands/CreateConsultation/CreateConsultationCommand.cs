using Shared.Application.Messaging;

namespace App.Application.Consultations.Commands.CreateConsultation;

public record CreateConsultationCommand(
    Guid ClientId,
    DateTime ConsultationDate,
    string Subject,
    string? Description,
    decimal? Fee,
    string? Notes,
    string CreatedBy) : ICommand<Guid>;
