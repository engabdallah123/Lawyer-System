using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.AddCaseTimeline;

internal sealed class AddCaseTimelineCommandHandler : ICommandHandler<AddCaseTimelineCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public AddCaseTimelineCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddCaseTimelineCommand request, CancellationToken cancellationToken)
    {
        var caseExists = await _unitOfWork.Cases.AnyAsync(c => c.Id == request.CaseId, cancellationToken);
        if (!caseExists)
            return Result<Guid>.Failure(CaseErrors.NotFound(request.CaseId));

        var timelineResult = CaseTimeline.Create(
            request.CaseId,
            request.Title,
            request.Description,
            request.IsImportant,
            request.CreatedBy);

        if (timelineResult.IsFailure)
            return Result<Guid>.Failure(timelineResult.Error);

        await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(timelineResult.Value!.Id);
    }
}
