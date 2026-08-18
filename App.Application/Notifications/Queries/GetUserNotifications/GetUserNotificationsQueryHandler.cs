using App.Application.Notifications.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Queries.GetUserNotifications
{
    internal sealed class GetUserNotificationsQueryHandler : IQueryHandler<GetUserNotificationsQuery, IEnumerable<NotificationDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUserNotificationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<NotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var sql = @"
            SELECT 
                Id,
                UserId,
                Title,
                Message,
                CASE Type 
                    WHEN 'System' THEN 0 
                    WHEN 'Email' THEN 1 
                    WHEN 'WhatsApp' THEN 2 
                    ELSE 0 
                END AS Type,
                CASE Type 
                    WHEN 'System' THEN N'نظام' 
                    WHEN 'Email' THEN N'بريد إلكتروني' 
                    WHEN 'WhatsApp' THEN N'واتساب' 
                    ELSE N'نظام' 
                END AS TypeName,
                IsRead,
                CreatedAt,
                ReadAt
            FROM Notifications
            WHERE (@UserId IS NULL OR @UserId = '' OR UserId = @UserId)
                AND (@OnlyUnread IS NULL OR (@OnlyUnread = 1 AND IsRead = 0))
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (request.Page - 1) * request.PageSize;

            var notifications = await connection.QueryAsync<NotificationDto>(
                sql,
                new
                {
                    UserId = request.UserId,
                    OnlyUnread = request.OnlyUnread,
                    Offset = offset,
                    PageSize = request.PageSize
                });

            return Result<IEnumerable<NotificationDto>>.Success(notifications);
        }
    }
}
