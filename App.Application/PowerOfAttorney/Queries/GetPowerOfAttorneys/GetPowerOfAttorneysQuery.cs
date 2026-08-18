using App.Application.PowerOfAttorney.DTOs;
using Shared.Application.Messaging;

namespace App.Application.PowerOfAttorney.Queries.GetPowerOfAttorneys;

public record GetPowerOfAttorneysQuery(
    Guid? ClientId = null,
    Guid? CaseId = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<PowerOfAttorneyDto>>;
