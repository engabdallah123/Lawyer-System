using App.Application.Consultations.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Consultations.Queries.GetConsultations;

public record GetConsultationsQuery(
    Guid? ClientId = null,
    int? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<ConsultationDto>>;
