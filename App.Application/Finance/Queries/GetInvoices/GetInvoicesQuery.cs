using App.Application.Finance.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Finance.Queries.GetInvoices;

public record GetInvoicesQuery(
    Guid? ClientId = null,
    Guid? CaseId = null,
    int? Status = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<InvoiceDto>>;
