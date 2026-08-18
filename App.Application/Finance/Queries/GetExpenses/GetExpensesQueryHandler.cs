using App.Application.Finance.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Queries.GetExpenses;

internal sealed class GetExpensesQueryHandler : IQueryHandler<GetExpensesQuery, IEnumerable<ExpenseDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetExpensesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<ExpenseDto>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                e.Id,
                e.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                e.ExpenseType,
                e.Amount,
                e.ExpenseDate,
                e.Description,
                e.ReceiptPath,
                e.PaidBy,
                e.CreatedAt,
                e.CreatedBy
            FROM Expenses e
            LEFT JOIN Cases c ON e.CaseId = c.Id
            WHERE (@CaseId IS NULL OR e.CaseId = @CaseId)
                AND (@FromDate IS NULL OR e.ExpenseDate >= @FromDate)
                AND (@ToDate IS NULL OR e.ExpenseDate <= @ToDate)
            ORDER BY e.ExpenseDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var expenses = await connection.QueryAsync<ExpenseDto>(
            sql,
            new
            {
                CaseId = request.CaseId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<ExpenseDto>>.Success(expenses);
    }
}
