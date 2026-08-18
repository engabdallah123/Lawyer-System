using App.Application.Cases.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Cases.Queries.GetCaseTimeline;

public record GetCaseTimelineQuery(Guid CaseId) : IQuery<IEnumerable<CaseTimelineDto>>;
