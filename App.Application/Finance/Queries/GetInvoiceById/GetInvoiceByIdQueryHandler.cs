using App.Application.Finance.DTOs;
using App.Domain.Finance.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Queries.GetInvoiceById;

internal sealed class GetInvoiceByIdQueryHandler : IQueryHandler<GetInvoiceByIdQuery, InvoiceDetailsDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetInvoiceByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<InvoiceDetailsDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var invoiceSql = @"
            SELECT 
                i.Id,
                i.InvoiceNumber,
                i.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                cl.Phone AS ClientPhone,
                cl.Address AS ClientAddress,
                i.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                i.FeeAgreementId,
                i.IssueDate,
                i.DueDate,
                i.SubTotal,
                i.Discount,
                i.TaxAmount,
                i.TotalAmount,
                i.PaidAmount,
                (i.TotalAmount - i.PaidAmount) AS RemainingAmount,
                CASE i.Status 
                    WHEN 'Draft' THEN 0 
                    WHEN 'Issued' THEN 1 
                    WHEN 'PartiallyPaid' THEN 2 
                    WHEN 'Paid' THEN 3 
                    WHEN 'Cancelled' THEN 4 
                    WHEN 'Overdue' THEN 5 
                    ELSE 0 
                END AS Status,
                CASE i.Status 
                    WHEN 'Draft' THEN N'مسودة' 
                    WHEN 'Issued' THEN N'صادرة' 
                    WHEN 'PartiallyPaid' THEN N'مدفوعة جزئيًا' 
                    WHEN 'Paid' THEN N'مدفوعة' 
                    WHEN 'Cancelled' THEN N'ملغاة' 
                    WHEN 'Overdue' THEN N'متأخرة' 
                    ELSE N'غير محدد' 
                END AS StatusName,
                i.Notes,
                i.QRCodePath,
                i.CreatedAt,
                i.CreatedBy
            FROM Invoices i
            INNER JOIN Clients cl ON i.ClientId = cl.Id
            LEFT JOIN Cases c ON i.CaseId = c.Id
            WHERE i.Id = @Id;";

        var itemsSql = @"
            SELECT Id, Description, Quantity, UnitPrice, Total
            FROM InvoiceItems
            WHERE InvoiceId = @Id;";

        var paymentsSql = @"
            SELECT 
                p.Id,
                p.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                p.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                p.FeeAgreementId,
                p.InvoiceId,
                inv.InvoiceNumber,
                p.Amount,
                p.PaymentDate,
                p.PaymentMethod,
                p.ReferenceNumber,
                p.Notes,
                p.ReceivedBy,
                p.CreatedAt
            FROM Payments p
            INNER JOIN Clients cl ON p.ClientId = cl.Id
            LEFT JOIN Cases c ON p.CaseId = c.Id
            LEFT JOIN Invoices inv ON p.InvoiceId = inv.Id
            WHERE p.InvoiceId = @Id
            ORDER BY p.PaymentDate DESC;";

        var invoice = await connection.QuerySingleOrDefaultAsync<InvoiceDetailsDto>(invoiceSql, new { Id = request.Id });
        if (invoice is null)
            return Result<InvoiceDetailsDto>.Failure(FinanceErrors.InvoiceNotFound(request.Id));

        var items = await connection.QueryAsync<InvoiceItemDto>(itemsSql, new { Id = request.Id });
        var payments = await connection.QueryAsync<PaymentDto>(paymentsSql, new { Id = request.Id });

        var fullInvoice = invoice with
        {
            Items = items,
            Payments = payments
        };

        return Result<InvoiceDetailsDto>.Success(fullInvoice);
    }
}
