using App.Application.PowerOfAttorney.DTOs;
using Shared.Application.Messaging;

namespace App.Application.PowerOfAttorney.Queries.GetExpiringPowerOfAttorneys;

public record GetExpiringPowerOfAttorneysQuery(int WithinDays = 30) : IQuery<IEnumerable<PowerOfAttorneyDto>>;
