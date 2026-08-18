namespace App.Application.Finance.DTOs;

public record ExpenseDto(
    Guid Id,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    string ExpenseType,
    decimal Amount,
    DateTime ExpenseDate,
    string? Description,
    string? ReceiptPath,
    string PaidBy,
    DateTime CreatedAt,
    string CreatedBy);
