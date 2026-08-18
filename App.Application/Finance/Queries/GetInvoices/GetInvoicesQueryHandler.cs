using App.Application.Finance.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Queries.GetInvoices;

internal sealed class GetInvoicesQueryHandler : IQueryHandler<GetInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetInvoicesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var statusName = request.Status switch
        {
            0 => "Draft",
            1 => "Issued",
            2 => "PartiallyPaid",
            3 => "Paid",
            4 => "Cancelled",
            5 => "Overdue",
            _ => null
        };

        var sql = @"
            SELECT 
                i.Id,
                i.InvoiceNumber,
                i.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
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
            WHERE (@ClientId IS NULL OR i.ClientId = @ClientId)
                AND (@CaseId IS NULL OR i.CaseId = @CaseId)
                AND (@Status IS NULL OR i.Status = @StatusName)
            ORDER BY i.IssueDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var invoices = await connection.QueryAsync<InvoiceDto>(
            sql,
            new
            {
                ClientId = request.ClientId,
                CaseId = request.CaseId,
                Status = request.Status,
                StatusName = statusName,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<InvoiceDto>>.Success(invoices);
    }
}
