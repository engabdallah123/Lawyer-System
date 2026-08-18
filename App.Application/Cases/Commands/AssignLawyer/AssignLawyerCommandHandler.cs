using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.AssignLawyer;

internal sealed class AssignLawyerCommandHandler : ICommandHandler<AssignLawyerCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public AssignLawyerCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AssignLawyerCommand request, CancellationToken cancellationToken)
    {
        var caseExists = await _unitOfWork.Cases.AnyAsync(c => c.Id == request.CaseId, cancellationToken);
        if (!caseExists)
            return Result<Guid>.Failure(CaseErrors.NotFound(request.CaseId));

        var assignmentResult = CaseAssignment.Create(
            request.CaseId,
            request.UserId,
            request.RoleInCase,
            request.AssignedDate,
            request.Notes);

        if (assignmentResult.IsFailure)
            return Result<Guid>.Failure(assignmentResult.Error);

        await _unitOfWork.CaseAssignments.AddAsync(assignmentResult.Value!, cancellationToken);

        var timelineResult = CaseTimeline.Create(
            request.CaseId,
            "تعيين محامٍ",
            $"تم تعيين المستخدم '{request.UserId}' بدور '{request.RoleInCase}'",
            false,
            request.AssignedBy);

        if (timelineResult.IsSuccess)
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(assignmentResult.Value!.Id);
    }
}
