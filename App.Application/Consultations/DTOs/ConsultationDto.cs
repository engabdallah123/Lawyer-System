using App.Domain.Consultations.Enums;

namespace App.Application.Consultations.DTOs;

public record ConsultationDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string? ClientPhone,
    DateTime ConsultationDate,
    string Subject,
    string? Description,
    decimal? Fee,
    ConsultationStatus Status,
    string StatusName,
    string? Notes,
    DateTime CreatedAt,
    string CreatedBy);
