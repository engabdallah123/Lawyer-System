using App.Domain;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.MarkAllNotificationsAsRead
{
    internal sealed class MarkAllNotificationsAsReadCommandHandler : ICommandHandler<MarkAllNotificationsAsReadCommand>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public MarkAllNotificationsAsReadCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var unreadNotifications = await _unitOfWork.Notifications.FindAllAsync(
                n => n.UserId == request.UserId && !n.IsRead, cancellationToken: cancellationToken);

            foreach (var notif in unreadNotifications)
            {
                notif.MarkAsRead();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
