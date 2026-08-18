using App.Application.Finance.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Queries.GetPayments;

internal sealed class GetPaymentsQueryHandler : IQueryHandler<GetPaymentsQuery, IEnumerable<PaymentDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPaymentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<PaymentDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
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
            WHERE (@ClientId IS NULL OR p.ClientId = @ClientId)
                AND (@CaseId IS NULL OR p.CaseId = @CaseId)
                AND (@InvoiceId IS NULL OR p.InvoiceId = @InvoiceId)
                AND (@FromDate IS NULL OR p.PaymentDate >= @FromDate)
                AND (@ToDate IS NULL OR p.PaymentDate <= @ToDate)
            ORDER BY p.PaymentDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var payments = await connection.QueryAsync<PaymentDto>(
            sql,
            new
            {
                ClientId = request.ClientId,
                CaseId = request.CaseId,
                InvoiceId = request.InvoiceId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<PaymentDto>>.Success(payments);
    }
}
