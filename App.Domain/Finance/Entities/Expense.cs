using App.Domain.Common;
using Shared.Domain;

namespace App.Domain.Finance.Entities;

/// <summary>
/// المصروف — يمكن ربطه بقضية
/// </summary>
public sealed class Expense : Entity, IAuditable
{
    public Guid? CaseId { get; private set; }
    public string ExpenseType { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public string? Description { get; private set; }
    public string? ReceiptPath { get; private set; }
    public string PaidBy { get; private set; } = null!;

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Cases.Entities.Case? Case { get; private set; }

    private Expense() { }

    private Expense(
        Guid id, Guid? caseId, string expenseType, decimal amount,
        DateTime expenseDate, string? description, string? receiptPath, string paidBy)
        : base(id)
    {
        CaseId = caseId;
        ExpenseType = expenseType;
        Amount = amount;
        ExpenseDate = expenseDate;
        Description = description;
        ReceiptPath = receiptPath;
        PaidBy = paidBy;
    }

    public static Result<Expense> Create(
        Guid? caseId, string expenseType, decimal amount,
        DateTime expenseDate, string? description, string? receiptPath, string paidBy)
    {
        if (amount <= 0)
            return Result<Expense>.Failure(Errors.FinanceErrors.InvalidAmount);

        if (string.IsNullOrWhiteSpace(expenseType))
            return Result<Expense>.Failure(Errors.FinanceErrors.ExpenseTypeRequired);

        var expense = new Expense(
            Guid.NewGuid(), caseId, expenseType.Trim(), amount,
            expenseDate, description?.Trim(), receiptPath, paidBy);

        return Result<Expense>.Success(expense);
    }

    public void SetReceiptPath(string path) => ReceiptPath = path;

    // IAuditable
    public void SetCreated(string createdBy) { CreatedAt = DateTime.UtcNow; CreatedBy = createdBy; }
    public void SetUpdated(string updatedBy) { UpdatedAt = DateTime.UtcNow; UpdatedBy = updatedBy; }
}
