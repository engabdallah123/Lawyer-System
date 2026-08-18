using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Hearings.Entities;
using App.Domain.Hearings.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Hearings.Commands.RecordHearingResult;

internal sealed class RecordHearingResultCommandHandler : ICommandHandler<RecordHearingResultCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public RecordHearingResultCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RecordHearingResultCommand request, CancellationToken cancellationToken)
    {
        var hearing = await _unitOfWork.Hearings.GetByIdAsync(request.HearingId, cancellationToken);
        if (hearing is null)
            return Result.Failure(HearingErrors.NotFound(request.HearingId));

        hearing.RecordResult(request.Result, request.Notes, request.NextHearingDate);
        hearing.SetUpdated(request.UpdatedBy);

        var resultText = !string.IsNullOrWhiteSpace(request.Result) ? $" — النتيجة: {request.Result}" : "";
        var nextDateText = request.NextHearingDate.HasValue ? $" وتم تحديد جلسة قادمة بتاريخ {request.NextHearingDate.Value:yyyy-MM-dd}" : "";

        var timelineResult = CaseTimeline.Create(
            hearing.CaseId,
            "تسجيل نتيجة الجلسة",
            $"تم تسجيل نتيجة جلسة {hearing.HearingDate:yyyy-MM-dd}{resultText}{nextDateText}",
            true,
            request.UpdatedBy);

        if (timelineResult.IsSuccess)
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);

        if (request.NextHearingDate.HasValue)
        {
            var nextHearingResult = Hearing.Create(
                hearing.CaseId,
                request.NextHearingDate.Value,
                request.NextHearingTime,
                request.NextHearingType ?? hearing.HearingType,
                "تم إنشاؤها تلقائيًا بناءً على قرار الجلسة السابقة");

            if (nextHearingResult.IsSuccess)
            {
                var nextHearing = nextHearingResult.Value!;
                nextHearing.SetCreated(request.UpdatedBy);
                await _unitOfWork.Hearings.AddAsync(nextHearing, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
