using App.Application.Hearings.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Hearings.Queries.GetHearingsByCase;

public record GetHearingsByCaseQuery(Guid CaseId) : IQuery<IEnumerable<HearingDto>>;
