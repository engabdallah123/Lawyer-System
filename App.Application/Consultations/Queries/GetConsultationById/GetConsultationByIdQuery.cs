using App.Application.Consultations.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Consultations.Queries.GetConsultationById;

public record GetConsultationByIdQuery(Guid Id) : IQuery<ConsultationDto>;
