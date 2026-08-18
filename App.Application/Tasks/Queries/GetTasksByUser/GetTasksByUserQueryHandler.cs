using App.Application.Tasks.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Queries.GetTasksByUser
{
    internal sealed class GetTasksByUserQueryHandler : IQueryHandler<GetTasksByUserQuery, IEnumerable<LegalTaskDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetTasksByUserQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<LegalTaskDto>>> Handle(GetTasksByUserQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var statusName = request.Status switch
            {
                0 => "Pending",
                1 => "InProgress",
                2 => "Completed",
                3 => "Cancelled",
                _ => null
            };

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
            WHERE (@UserId IS NULL OR @UserId = '' OR t.AssignedToUserId = @UserId)
                AND (@Status IS NULL OR t.Status = @StatusName)
            ORDER BY t.DueDate ASC, t.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (request.Page - 1) * request.PageSize;

            var tasks = await connection.QueryAsync<LegalTaskDto>(
                sql,
                new
                {
                    UserId = request.UserId,
                    Status = request.Status,
                    StatusName = statusName,
                    Offset = offset,
                    PageSize = request.PageSize
                });

            return Result<IEnumerable<LegalTaskDto>>.Success(tasks);
        }
    }
}
