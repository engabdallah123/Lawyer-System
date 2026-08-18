using App.Application.Cases.DTOs;
using App.Application.Dashboard.DTOs;
using App.Application.Hearings.DTOs;
using App.Application.Tasks.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Dashboard.Queries.GetLawerDashboard
{
    internal sealed class GetLawyerDashboardQueryHandler : IQueryHandler<GetLawyerDashboardQuery, LawyerDashboardDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetLawyerDashboardQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<LawyerDashboardDto>> Handle(GetLawyerDashboardQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            // 1. جلسات اليوم
            var todayHearingsSql = @"
            SELECT 
                h.Id, h.CaseId, c.InternalNumber AS CaseInternalNumber, c.Title AS CaseTitle,
                crt.Name AS CourtName, c.Circuit, h.HearingDate, h.HearingTime,
                h.HearingType, h.Result, h.Notes, h.NextHearingDate, h.CreatedAt, h.CreatedBy
            FROM Hearings h
            INNER JOIN Cases c ON h.CaseId = c.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            INNER JOIN CaseAssignments ca ON c.Id = ca.CaseId
            WHERE c.IsDeleted = 0
                AND ca.UserId = @UserId
                AND CAST(h.HearingDate AS DATE) = CAST(GETUTCDATE() AS DATE)
            ORDER BY h.HearingTime ASC;";

            // 2. جلسات هذا الأسبوع
            var weekHearingsSql = @"
            SELECT 
                h.Id, h.CaseId, c.InternalNumber AS CaseInternalNumber, c.Title AS CaseTitle,
                crt.Name AS CourtName, c.Circuit, h.HearingDate, h.HearingTime,
                h.HearingType, h.Result, h.Notes, h.NextHearingDate, h.CreatedAt, h.CreatedBy
            FROM Hearings h
            INNER JOIN Cases c ON h.CaseId = c.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            INNER JOIN CaseAssignments ca ON c.Id = ca.CaseId
            WHERE c.IsDeleted = 0
                AND ca.UserId = @UserId
                AND h.HearingDate >= CAST(GETUTCDATE() AS DATE)
                AND h.HearingDate <= DATEADD(day, 7, CAST(GETUTCDATE() AS DATE))
            ORDER BY h.HearingDate ASC, h.HearingTime ASC;";

            // 3. القضايا المسندة
            var assignedCasesSql = @"
            SELECT 
                c.Id, c.InternalNumber, c.CourtNumber, c.Title,
                c.CaseTypeId, ct.Name AS CaseTypeName,
                c.CaseStatusId, cs.Name AS CaseStatusName, cs.Color AS CaseStatusColor,
                c.CourtId, crt.Name AS CourtName, c.Circuit, c.JudgeName,
                c.OpenDate, c.CloseDate, c.ClaimAmount, c.CurrentStage, c.CreatedAt
            FROM Cases c
            INNER JOIN CaseAssignments ca ON c.Id = ca.CaseId
            INNER JOIN CaseTypes ct ON c.CaseTypeId = ct.Id
            INNER JOIN CaseStatuses cs ON c.CaseStatusId = cs.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.IsDeleted = 0 AND c.CloseDate IS NULL
                AND ca.UserId = @UserId
            ORDER BY c.OpenDate DESC;";

            // 4. المهام المتأخرة
            var overdueTasksSql = @"
            SELECT 
                t.Id, t.CaseId, c.InternalNumber AS CaseInternalNumber, c.Title AS CaseTitle,
                t.AssignedToUserId, t.Title, t.Description, t.DueDate,
                CASE t.Priority WHEN 'Low' THEN 0 WHEN 'Normal' THEN 1 WHEN 'High' THEN 2 WHEN 'Urgent' THEN 3 ELSE 1 END AS Priority,
                t.Priority AS PriorityName,
                CASE t.Status WHEN 'Pending' THEN 0 WHEN 'InProgress' THEN 1 WHEN 'Completed' THEN 2 WHEN 'Cancelled' THEN 3 ELSE 0 END AS Status,
                t.Status AS StatusName,
                t.CompletedAt, t.CreatedAt, t.CreatedBy
            FROM LegalTasks t
            LEFT JOIN Cases c ON t.CaseId = c.Id
            WHERE t.AssignedToUserId = @UserId
                AND t.Status IN ('Pending', 'InProgress')
                AND t.DueDate < GETUTCDATE()
            ORDER BY t.DueDate ASC;";

            // 5. المهام القادمة
            var upcomingTasksSql = @"
            SELECT 
                t.Id, t.CaseId, c.InternalNumber AS CaseInternalNumber, c.Title AS CaseTitle,
                t.AssignedToUserId, t.Title, t.Description, t.DueDate,
                CASE t.Priority WHEN 'Low' THEN 0 WHEN 'Normal' THEN 1 WHEN 'High' THEN 2 WHEN 'Urgent' THEN 3 ELSE 1 END AS Priority,
                t.Priority AS PriorityName,
                CASE t.Status WHEN 'Pending' THEN 0 WHEN 'InProgress' THEN 1 WHEN 'Completed' THEN 2 WHEN 'Cancelled' THEN 3 ELSE 0 END AS Status,
                t.Status AS StatusName,
                t.CompletedAt, t.CreatedAt, t.CreatedBy
            FROM LegalTasks t
            LEFT JOIN Cases c ON t.CaseId = c.Id
            WHERE t.AssignedToUserId = @UserId
                AND t.Status IN ('Pending', 'InProgress')
                AND (t.DueDate >= GETUTCDATE() OR t.DueDate IS NULL)
            ORDER BY t.DueDate ASC;";

            // 6. آخر التحديثات على قضايا المحامي
            var recentUpdatesSql = @"
            SELECT TOP (10)
                ct.Id, ct.CaseId, ct.Title, ct.Description, ct.IsImportant, ct.CreatedAt, ct.CreatedBy
            FROM CaseTimelines ct
            INNER JOIN CaseAssignments ca ON ct.CaseId = ca.CaseId
            WHERE ca.UserId = @UserId
            ORDER BY ct.CreatedAt DESC;";

            // 7. عدد الإشعارات غير المقروءة
            var unreadNotifSql = @"SELECT COUNT(*) FROM Notifications WHERE UserId = @UserId AND IsRead = 0;";

            var todayHearings = await connection.QueryAsync<HearingDto>(todayHearingsSql, new { UserId = request.LawyerUserId });
            var weekHearings = await connection.QueryAsync<HearingDto>(weekHearingsSql, new { UserId = request.LawyerUserId });
            var assignedCases = await connection.QueryAsync<CaseDto>(assignedCasesSql, new { UserId = request.LawyerUserId });
            var overdueTasks = await connection.QueryAsync<LegalTaskDto>(overdueTasksSql, new { UserId = request.LawyerUserId });
            var upcomingTasks = await connection.QueryAsync<LegalTaskDto>(upcomingTasksSql, new { UserId = request.LawyerUserId });
            var recentUpdates = await connection.QueryAsync<CaseTimelineDto>(recentUpdatesSql, new { UserId = request.LawyerUserId });
            var unreadCount = await connection.ExecuteScalarAsync<int>(unreadNotifSql, new { UserId = request.LawyerUserId });

            var dashboard = new LawyerDashboardDto(
                todayHearings,
                weekHearings,
                assignedCases,
                overdueTasks,
                upcomingTasks,
                recentUpdates,
                unreadCount);

            return Result<LawyerDashboardDto>.Success(dashboard);
        }
    }
}
