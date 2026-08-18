using App.Application.Finance.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Finance.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid Id) : IQuery<InvoiceDetailsDto>;
