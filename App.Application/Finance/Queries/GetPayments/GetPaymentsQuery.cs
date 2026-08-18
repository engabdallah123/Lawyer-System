using App.Application.Finance.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Finance.Queries.GetPayments;

public record GetPaymentsQuery(
    Guid? ClientId = null,
    Guid? CaseId = null,
    Guid? InvoiceId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<PaymentDto>>;
