using App.Application.Finance.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Finance.Queries.GetExpenses;

public record GetExpensesQuery(
    Guid? CaseId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<ExpenseDto>>;
