using App.Application.Finance.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Finance.Queries.GetFinancialSummary;

public record GetFinancialSummaryQuery(DateTime? FromDate = null, DateTime? ToDate = null) : IQuery<FinancialSummaryDto>;
