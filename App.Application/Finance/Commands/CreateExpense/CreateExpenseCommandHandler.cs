using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Finance.Entities;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Commands.CreateExpense;

internal sealed class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseResult = Expense.Create(
            request.CaseId,
            request.ExpenseType,
            request.Amount,
            request.ExpenseDate,
            request.Description,
            request.ReceiptPath,
            request.PaidBy);

        if (expenseResult.IsFailure)
            return Result<Guid>.Failure(expenseResult.Error);

        var expense = expenseResult.Value!;
        expense.SetCreated(request.CreatedBy);

        await _unitOfWork.Expenses.AddAsync(expense, cancellationToken);

        if (request.CaseId.HasValue)
        {
            var timelineResult = CaseTimeline.Create(
                request.CaseId.Value,
                "تسجيل مصروف",
                $"تم تسجيل مصروف ({request.ExpenseType}) بقيمة {request.Amount:N2}",
                false,
                request.CreatedBy);

            if (timelineResult.IsSuccess)
                await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(expense.Id);
    }
}
