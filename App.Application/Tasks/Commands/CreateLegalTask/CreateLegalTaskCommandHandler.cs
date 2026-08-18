using App.Domain;
using App.Domain.Notifications.Entities;
using App.Domain.Notifications.Enums;
using App.Domain.Tasks.Entities;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.CreateLegalTask
{
    internal sealed class CreateLegalTaskCommandHandler : ICommandHandler<CreateLegalTaskCommand, Guid>
    {
        private readonly IAppUnitOfWork _unitOfWork;

        public CreateLegalTaskCommandHandler(IAppUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateLegalTaskCommand request, CancellationToken cancellationToken)
        {
            var taskResult = LegalTask.Create(
                request.CaseId,
                request.AssignedToUserId,
                request.Title,
                request.Description,
                request.DueDate,
                request.Priority);

            if (taskResult.IsFailure)
                return Result<Guid>.Failure(taskResult.Error);

            var task = taskResult.Value!;
            task.SetCreated(request.CreatedBy);

            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);

            // إرسال إشعار للمستخدم المسند إليه المهمة
            var notificationResult = Notification.Create(
                request.AssignedToUserId,
                "مهمة جديدة",
                $"تم إسناد مهمة جديدة إليك: '{request.Title}'",
                NotificationType.System);

            if (notificationResult.IsSuccess)
                await _unitOfWork.Notifications.AddAsync(notificationResult.Value!, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(task.Id);
        }
    }
}
