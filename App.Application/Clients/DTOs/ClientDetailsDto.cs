using App.Domain.Clients.Enums;

namespace App.Application.Clients.DTOs;

public record ClientDetailsDto(
    Guid Id,
    ClientType ClientType,
    string? FullName,
    string? CompanyName,
    string? NationalId,
    string? CommercialRegister,
    string Phone,
    string? Mobile,
    string? Email,
    string? Address,
    string? City,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy,
    int ActiveCasesCount,
    int TotalConsultationsCount,
    int TotalPowerOfAttorneysCount);
