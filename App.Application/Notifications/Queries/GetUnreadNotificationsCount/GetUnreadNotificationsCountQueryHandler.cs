using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Queries.GetUnreadNotificationsCount
{
    internal sealed class GetUnreadNotificationsCountQueryHandler : IQueryHandler<GetUnreadNotificationsCountQuery, int>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUnreadNotificationsCountQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<int>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var sql = @"
            SELECT COUNT(*)
            FROM Notifications
            WHERE UserId = @UserId AND IsRead = 0;";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = request.UserId });
            return Result<int>.Success(count);
        }
    }
}
