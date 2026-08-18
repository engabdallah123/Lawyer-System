using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.CloseCase;

internal sealed class CloseCaseCommandHandler : ICommandHandler<CloseCaseCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CloseCaseCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CloseCaseCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await _unitOfWork.Cases.GetByIdAsync(request.CaseId, cancellationToken);
        if (caseEntity is null)
            return Result.Failure(CaseErrors.NotFound(request.CaseId));

        if (caseEntity.CloseDate.HasValue)
            return Result.Failure(CaseErrors.AlreadyClosed);

        caseEntity.CloseCase(request.CloseDate);
        caseEntity.SetUpdated(request.ClosedBy);

        var timelineResult = CaseTimeline.Create(
            caseEntity.Id,
            "تم إغلاق القضية",
            $"تم إغلاق القضية بتاريخ {request.CloseDate:yyyy-MM-dd}",
            true,
            request.ClosedBy);

        if (timelineResult.IsSuccess)
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
