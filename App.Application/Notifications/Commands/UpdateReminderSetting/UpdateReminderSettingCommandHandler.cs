using App.Domain;
using App.Domain.Notifications.Entities;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.UpdateReminderSetting
{
    internal sealed class UpdateReminderSettingCommandHandler : ICommandHandler<UpdateReminderSettingCommand>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public UpdateReminderSettingCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateReminderSettingCommand request, CancellationToken cancellationToken)
        {
            var setting = await _unitOfWork.ReminderSettings.FindAsync(
                r => r.UserId == request.UserId, cancellationToken: cancellationToken);

            if (setting is null)
            {
                setting = ReminderSetting.Create(
                    request.UserId,
                    request.DaysBeforeHearing,
                    request.NotifyBySystem,
                    request.NotifyByEmail,
                    request.NotifyByWhatsApp);

                await _unitOfWork.ReminderSettings.AddAsync(setting, cancellationToken);
            }
            else
            {
                setting.Update(
                    request.DaysBeforeHearing,
                    request.NotifyBySystem,
                    request.NotifyByEmail,
                    request.NotifyByWhatsApp);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
