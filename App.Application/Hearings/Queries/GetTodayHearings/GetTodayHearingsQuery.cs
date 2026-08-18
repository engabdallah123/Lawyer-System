using App.Application.Hearings.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Hearings.Queries.GetTodayHearings;

public record GetTodayHearingsQuery(string? LawyerUserId = null) : IQuery<IEnumerable<HearingDto>>;
