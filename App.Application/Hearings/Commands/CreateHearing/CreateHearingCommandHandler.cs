using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Errors;
using App.Domain.Hearings.Entities;
using App.Domain.Notifications.Entities;
using App.Domain.Notifications.Enums;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Commands.CreateHearing;

internal sealed class CreateHearingCommandHandler : ICommandHandler<CreateHearingCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateHearingCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateHearingCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _unitOfWork.Cases.GetByIdAsync(request.CaseId, cancellationToken);
        if (caseEntity is null)
            return Result<Guid>.Failure(CaseErrors.NotFound(request.CaseId));

        var hearingResult = Hearing.Create(
            request.CaseId,
            request.HearingDate,
            request.HearingTime,
            request.HearingType,
            request.Notes);

        if (hearingResult.IsFailure)
            return Result<Guid>.Failure(hearingResult.Error);

        var hearing = hearingResult.Value!;
        hearing.SetCreated(request.CreatedBy);

        await _unitOfWork.Hearings.AddAsync(hearing, cancellationToken);

        // إضافة حدث في سجل تطورات القضية
        var timeFormatted = request.HearingTime.HasValue ? $" الساعة {request.HearingTime.Value:hh\\:mm}" : "";
        var timelineResult = CaseTimeline.Create(
            request.CaseId,
            "تحديد جلسة جديدة",
            $"تم تحديد جلسة ({request.HearingType}) بتاريخ {request.HearingDate:yyyy-MM-dd}{timeFormatted}",
            true,
            request.CreatedBy);

        if (timelineResult.IsSuccess)
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);

        // إرسال إشعارات للمحامين المعينين في القضية
        var assignments = await _unitOfWork.CaseAssignments.FindAllAsync(
            ca => ca.CaseId == request.CaseId, cancellationToken: cancellationToken);

        foreach (var assignment in assignments)
        {
            var notificationResult = Notification.Create(
                assignment.UserId,
                "جلسة جديدة",
                $"تم تحديد جلسة للقضية '{caseEntity.Title}' بتاريخ {request.HearingDate:yyyy-MM-dd}",
                NotificationType.System);

            if (notificationResult.IsSuccess)
                await _unitOfWork.Notifications.AddAsync(notificationResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(hearing.Id);
    }
}
