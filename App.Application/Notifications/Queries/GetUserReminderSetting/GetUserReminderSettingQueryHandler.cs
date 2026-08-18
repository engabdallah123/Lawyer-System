using App.Application.Notifications.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Queries.GetUserReminderSetting
{
    internal sealed class GetUserReminderSettingQueryHandler : IQueryHandler<GetUserReminderSettingQuery, ReminderSettingDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUserReminderSettingQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<ReminderSettingDto>> Handle(GetUserReminderSettingQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var sql = @"
            SELECT Id, UserId, DaysBeforeHearing, NotifyBySystem, NotifyByEmail, NotifyByWhatsApp
            FROM ReminderSettings
            WHERE UserId = @UserId;";

            var setting = await connection.QuerySingleOrDefaultAsync<ReminderSettingDto>(sql, new { UserId = request.UserId });

            if (setting is null)
            {
                // إرجاع الإعدادات الافتراضية
                setting = new ReminderSettingDto(Guid.Empty, request.UserId, 1, true, true, false);
            }

            return Result<ReminderSettingDto>.Success(setting);
        }
    }
}
