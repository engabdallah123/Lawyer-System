using App.Application.PowerOfAttorney.DTOs;
using Shared.Application.Messaging;

namespace App.Application.PowerOfAttorney.Queries.GetPowerOfAttorneyById;

public record GetPowerOfAttorneyByIdQuery(Guid Id) : IQuery<PowerOfAttorneyDto>;
