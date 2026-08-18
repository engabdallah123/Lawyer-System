namespace App.Application.Finance.DTOs;

public record FinancialSummaryDto(
    decimal TotalFeeAgreementsAmount,
    decimal TotalCollectedPayments,
    decimal TotalOutstandingReceivables,
    decimal TotalExpenses,
    decimal NetRevenue,
    int TotalInvoicesCount,
    int PaidInvoicesCount,
    int OverdueInvoicesCount);
