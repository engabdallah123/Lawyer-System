using App.Domain;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.MarkNotificationAsRead
{
    internal sealed class MarkNotificationAsReadCommandHandler : ICommandHandler<MarkNotificationAsReadCommand>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public MarkNotificationAsReadCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notif = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken);
            if (notif is null)
                return Result.Failure(new Error("Notification.NotFound", "الإشعار غير موجود."));

            notif.MarkAsRead();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
