using App.Domain;
using App.Domain.Notifications.Entities;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Notifications.Commands.CreateNotification
{
    internal sealed class CreateNotificationCommandHandler : ICommandHandler<CreateNotificationCommand, Guid>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public CreateNotificationCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var notifResult = Notification.Create(
                request.UserId,
                request.Title,
                request.Message,
                request.Type);

            if (notifResult.IsFailure)
                return Result<Guid>.Failure(notifResult.Error);

            var notification = notifResult.Value!;
            await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(notification.Id);
        }
    }
}
