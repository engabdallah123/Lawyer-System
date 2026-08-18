using App.Application.Tasks.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Queries.GetOverdueTasks
{
    internal sealed class GetOverdueTasksQueryHandler : IQueryHandler<GetOverdueTasksQuery, IEnumerable<LegalTaskDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetOverdueTasksQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<LegalTaskDto>>> Handle(GetOverdueTasksQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var sql = @"
            SELECT 
                t.Id,
                t.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                t.AssignedToUserId,
                t.Title,
                t.Description,
                t.DueDate,
                CASE t.Priority 
                    WHEN 'Low' THEN 0 
                    WHEN 'Normal' THEN 1 
                    WHEN 'High' THEN 2 
                    WHEN 'Urgent' THEN 3 
                    ELSE 1 
                END AS Priority,
                CASE t.Priority 
                    WHEN 'Low' THEN N'منخفضة' 
                    WHEN 'Normal' THEN N'عادية' 
                    WHEN 'High' THEN N'عالية' 
                    WHEN 'Urgent' THEN N'عاجلة' 
                    ELSE N'عادية' 
                END AS PriorityName,
                CASE t.Status 
                    WHEN 'Pending' THEN 0 
                    WHEN 'InProgress' THEN 1 
                    WHEN 'Completed' THEN 2 
                    WHEN 'Cancelled' THEN 3 
                    ELSE 0 
                END AS Status,
                CASE t.Status 
                    WHEN 'Pending' THEN N'معلقة' 
                    WHEN 'InProgress' THEN N'قيد التنفيذ' 
                    WHEN 'Completed' THEN N'مكتملة' 
                    WHEN 'Cancelled' THEN N'ملغاة' 
                    ELSE N'غير محدد' 
                END AS StatusName,
                t.CompletedAt,
                t.CreatedAt,
                t.CreatedBy
            FROM LegalTasks t
            LEFT JOIN Cases c ON t.CaseId = c.Id
            WHERE t.Status IN ('Pending', 'InProgress')
                AND t.DueDate < GETUTCDATE()
                AND (@UserId IS NULL OR t.AssignedToUserId = @UserId)
            ORDER BY t.DueDate ASC;";

            var tasks = await connection.QueryAsync<LegalTaskDto>(sql, new { UserId = request.UserId });
            return Result<IEnumerable<LegalTaskDto>>.Success(tasks);
        }
    }
}
