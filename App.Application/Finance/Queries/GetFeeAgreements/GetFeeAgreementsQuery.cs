using App.Application.Finance.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Finance.Queries.GetFeeAgreements;

public record GetFeeAgreementsQuery(
    Guid? ClientId = null,
    Guid? CaseId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<FeeAgreementDto>>;
