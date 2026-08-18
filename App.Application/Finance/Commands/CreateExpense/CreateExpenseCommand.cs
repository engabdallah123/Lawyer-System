using Shared.Application.Messaging;

namespace App.Application.Finance.Commands.CreateExpense;

public record CreateExpenseCommand(
    Guid? CaseId,
    string ExpenseType,
    decimal Amount,
    DateTime ExpenseDate,
    string? Description,
    string? ReceiptPath,
    string PaidBy,
    string CreatedBy) : ICommand<Guid>;
