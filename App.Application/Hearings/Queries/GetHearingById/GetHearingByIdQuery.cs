using App.Application.Hearings.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Hearings.Queries.GetHearingById;

public record GetHearingByIdQuery(Guid Id) : IQuery<HearingDto>;
