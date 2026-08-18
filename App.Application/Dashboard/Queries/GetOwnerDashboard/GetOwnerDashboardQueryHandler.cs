using App.Application.Cases.DTOs;
using App.Application.Dashboard.DTOs;
using App.Application.Hearings.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Dashboard.Queries.GetOwnerDashboard
{
    internal sealed class GetOwnerDashboardQueryHandler : IQueryHandler<GetOwnerDashboardQuery, OwnerDashboardDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetOwnerDashboardQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<OwnerDashboardDto>> Handle(GetOwnerDashboardQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var statsSql = @"
            SELECT 
                (SELECT COUNT(*) FROM Cases WHERE IsDeleted = 0) AS TotalCasesCount,
                (SELECT COUNT(*) FROM Cases WHERE IsDeleted = 0 AND CloseDate IS NULL) AS OpenCasesCount,
                (SELECT COUNT(*) FROM Cases WHERE IsDeleted = 0 AND CloseDate IS NOT NULL) AS ClosedCasesCount,
                (SELECT COUNT(*) FROM Hearings h INNER JOIN Cases c ON h.CaseId = c.Id WHERE c.IsDeleted = 0 AND CAST(h.HearingDate AS DATE) = CAST(GETUTCDATE() AS DATE)) AS TodayHearingsCount,
                (SELECT COUNT(*) FROM Hearings h INNER JOIN Cases c ON h.CaseId = c.Id WHERE c.IsDeleted = 0 AND h.HearingDate >= CAST(GETUTCDATE() AS DATE) AND h.HearingDate <= DATEADD(day, 7, CAST(GETUTCDATE() AS DATE))) AS WeekHearingsCount,
                ISNULL((SELECT SUM(TotalAmount) FROM FeeAgreements), 0) AS TotalAgreedFees,
                ISNULL((SELECT SUM(Amount) FROM Payments), 0) AS TotalCollectedFees,
                ISNULL((SELECT SUM(TotalAmount - PaidAmount) FROM FeeAgreements), 0) AS TotalOutstandingReceivables,
                ISNULL((SELECT SUM(Amount) FROM Expenses), 0) AS TotalExpenses,
                (ISNULL((SELECT SUM(Amount) FROM Payments), 0) - ISNULL((SELECT SUM(Amount) FROM Expenses), 0)) AS NetRevenue,
                (SELECT COUNT(*) FROM Clients WHERE IsDeleted = 0 AND IsActive = 1) AS TotalActiveClientsCount,
                (SELECT COUNT(*) FROM PowerOfAttorneys WHERE IsActive = 1 AND ExpiryDate IS NOT NULL AND ExpiryDate <= DATEADD(day, 30, GETUTCDATE()) AND ExpiryDate >= GETUTCDATE()) AS ExpiringPoasCount;";

            var casesByTypeSql = @"
            SELECT ct.Name AS TypeName, COUNT(c.Id) AS CasesCount
            FROM CaseTypes ct
            LEFT JOIN Cases c ON ct.Id = c.CaseTypeId AND c.IsDeleted = 0
            GROUP BY ct.Name
            ORDER BY CasesCount DESC;";

            var lawyerPerformanceSql = @"
            SELECT 
                ca.UserId,
                COUNT(DISTINCT CASE WHEN c.CloseDate IS NULL THEN c.Id END) AS ActiveCasesCount,
                COUNT(DISTINCT CASE WHEN c.CloseDate IS NOT NULL THEN c.Id END) AS ClosedCasesCount,
                COUNT(DISTINCT h.Id) AS TotalHearingsCount,
                COUNT(DISTINCT CASE WHEN t.Status = 'Completed' THEN t.Id END) AS CompletedTasksCount,
                COUNT(DISTINCT CASE WHEN t.Status IN ('Pending', 'InProgress') THEN t.Id END) AS PendingTasksCount
            FROM CaseAssignments ca
            INNER JOIN Cases c ON ca.CaseId = c.Id AND c.IsDeleted = 0
            LEFT JOIN Hearings h ON c.Id = h.CaseId
            LEFT JOIN LegalTasks t ON ca.UserId = t.AssignedToUserId
            GROUP BY ca.UserId;";

            var todayHearingsSql = @"
            SELECT 
                h.Id, h.CaseId, c.InternalNumber AS CaseInternalNumber, c.Title AS CaseTitle,
                crt.Name AS CourtName, c.Circuit, h.HearingDate, h.HearingTime,
                h.HearingType, h.Result, h.Notes, h.NextHearingDate, h.CreatedAt, h.CreatedBy
            FROM Hearings h
            INNER JOIN Cases c ON h.CaseId = c.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.IsDeleted = 0
                AND CAST(h.HearingDate AS DATE) = CAST(GETUTCDATE() AS DATE)
            ORDER BY h.HearingTime ASC;";

            var recentUpdatesSql = @"
            SELECT TOP (15)
                ct.Id, ct.CaseId, ct.Title, ct.Description, ct.IsImportant, ct.CreatedAt, ct.CreatedBy
            FROM CaseTimelines ct
            INNER JOIN Cases c ON ct.CaseId = c.Id
            WHERE c.IsDeleted = 0
            ORDER BY ct.CreatedAt DESC;";

            var stats = await connection.QuerySingleAsync<dynamic>(statsSql);
            var casesByType = await connection.QueryAsync<CaseTypeStatDto>(casesByTypeSql);
            var lawyerPerformance = await connection.QueryAsync<LawyerPerformanceStatDto>(lawyerPerformanceSql);
            var todayHearings = await connection.QueryAsync<HearingDto>(todayHearingsSql);
            var recentUpdates = await connection.QueryAsync<CaseTimelineDto>(recentUpdatesSql);

            var ownerDashboard = new OwnerDashboardDto(
                (int)stats.TotalCasesCount,
                (int)stats.OpenCasesCount,
                (int)stats.ClosedCasesCount,
                (int)stats.TodayHearingsCount,
                (int)stats.WeekHearingsCount,
                (decimal)stats.TotalAgreedFees,
                (decimal)stats.TotalCollectedFees,
                (decimal)stats.TotalOutstandingReceivables,
                (decimal)stats.TotalExpenses,
                (decimal)stats.NetRevenue,
                (int)stats.TotalActiveClientsCount,
                (int)stats.ExpiringPoasCount,
                casesByType,
                lawyerPerformance,
                todayHearings,
                recentUpdates);

            return Result<OwnerDashboardDto>.Success(ownerDashboard);
        }
    }
}
