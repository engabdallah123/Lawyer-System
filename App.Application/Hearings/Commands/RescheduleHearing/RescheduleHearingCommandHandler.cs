using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Hearings.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Commands.RescheduleHearing;

internal sealed class RescheduleHearingCommandHandler : ICommandHandler<RescheduleHearingCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public RescheduleHearingCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RescheduleHearingCommand request, CancellationToken cancellationToken)
    {
        var hearing = await _unitOfWork.Hearings.GetByIdAsync(request.HearingId, cancellationToken);
        if (hearing is null)
            return Result.Failure(HearingErrors.NotFound(request.HearingId));

        var oldDate = hearing.HearingDate;
        hearing.Reschedule(request.NewDate, request.NewTime);
        hearing.SetUpdated(request.RescheduledBy);

        var timelineResult = CaseTimeline.Create(
            hearing.CaseId,
            "تعديل موعد الجلسة",
            $"تم تعديل موعد الجلسة من {oldDate:yyyy-MM-dd} إلى {request.NewDate:yyyy-MM-dd}",
            true,
            request.RescheduledBy);

        if (timelineResult.IsSuccess)
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
