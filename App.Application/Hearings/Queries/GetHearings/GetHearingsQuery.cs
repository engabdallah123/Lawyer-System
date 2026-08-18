using App.Application.Hearings.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Hearings.Queries.GetHearings;

public record GetHearingsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? CaseId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<HearingDto>>;
