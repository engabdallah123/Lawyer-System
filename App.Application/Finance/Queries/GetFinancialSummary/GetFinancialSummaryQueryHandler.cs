using App.Application.Finance.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Queries.GetFinancialSummary;

internal sealed class GetFinancialSummaryQueryHandler : IQueryHandler<GetFinancialSummaryQuery, FinancialSummaryDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetFinancialSummaryQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<FinancialSummaryDto>> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                ISNULL((SELECT SUM(TotalAmount) FROM FeeAgreements WHERE (@FromDate IS NULL OR CreatedAt >= @FromDate) AND (@ToDate IS NULL OR CreatedAt <= @ToDate)), 0) AS TotalFeeAgreementsAmount,
                ISNULL((SELECT SUM(Amount) FROM Payments WHERE (@FromDate IS NULL OR PaymentDate >= @FromDate) AND (@ToDate IS NULL OR PaymentDate <= @ToDate)), 0) AS TotalCollectedPayments,
                ISNULL((SELECT SUM(TotalAmount - PaidAmount) FROM FeeAgreements WHERE (@FromDate IS NULL OR CreatedAt >= @FromDate) AND (@ToDate IS NULL OR CreatedAt <= @ToDate)), 0) AS TotalOutstandingReceivables,
                ISNULL((SELECT SUM(Amount) FROM Expenses WHERE (@FromDate IS NULL OR ExpenseDate >= @FromDate) AND (@ToDate IS NULL OR ExpenseDate <= @ToDate)), 0) AS TotalExpenses,
                (ISNULL((SELECT SUM(Amount) FROM Payments WHERE (@FromDate IS NULL OR PaymentDate >= @FromDate) AND (@ToDate IS NULL OR PaymentDate <= @ToDate)), 0) -
                 ISNULL((SELECT SUM(Amount) FROM Expenses WHERE (@FromDate IS NULL OR ExpenseDate >= @FromDate) AND (@ToDate IS NULL OR ExpenseDate <= @ToDate)), 0)) AS NetRevenue,
                (SELECT COUNT(*) FROM Invoices) AS TotalInvoicesCount,
                (SELECT COUNT(*) FROM Invoices WHERE Status = 'Paid') AS PaidInvoicesCount,
                (SELECT COUNT(*) FROM Invoices WHERE Status = 'Overdue' OR (DueDate < GETUTCDATE() AND Status IN ('Issued', 'PartiallyPaid'))) AS OverdueInvoicesCount;";

        var summary = await connection.QuerySingleAsync<FinancialSummaryDto>(
            sql,
            new { FromDate = request.FromDate, ToDate = request.ToDate });

        return Result<FinancialSummaryDto>.Success(summary);
    }
}
