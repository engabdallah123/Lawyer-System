using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Enums;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.CreateCase;

internal sealed class CreateCaseCommandHandler : ICommandHandler<CreateCaseCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateCaseCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        var caseResult = Case.Create(
            request.InternalNumber,
            request.CourtNumber,
            request.Title,
            request.CaseTypeId,
            request.CaseStatusId,
            request.CourtId,
            request.Circuit,
            request.JudgeName,
            request.OpenDate,
            request.ClaimAmount,
            request.Description,
            request.CurrentStage,
            request.Notes);

        if (caseResult.IsFailure)
            return Result<Guid>.Failure(caseResult.Error);

        var caseEntity = caseResult.Value!;
        caseEntity.SetCreated(request.CreatedBy);

        await _unitOfWork.Cases.AddAsync(caseEntity, cancellationToken);

        // إضافة الموكل الرئيسي كطرف في القضية إذا حُدد
        if (request.MainClientId.HasValue)
        {
            var partyResult = CaseParty.Create(
                caseEntity.Id,
                request.MainClientId.Value,
                null,
                PartyRole.Plaintiff,
                true,
                "الموكل الرئيسي");

            if (partyResult.IsSuccess)
            {
                await _unitOfWork.CaseParties.AddAsync(partyResult.Value!, cancellationToken);
            }
        }

        // إضافة المحامي الرئيسي إذا حُدد
        if (!string.IsNullOrWhiteSpace(request.MainLawyerUserId))
        {
            var assignmentResult = CaseAssignment.Create(
                caseEntity.Id,
                request.MainLawyerUserId,
                "محامي أساسي",
                DateTime.UtcNow,
                "تم التعيين عند فتح القضية");

            if (assignmentResult.IsSuccess)
            {
                await _unitOfWork.CaseAssignments.AddAsync(assignmentResult.Value!, cancellationToken);
            }
        }

        // إضافة مدخل Timeline تلقائي
        var timelineResult = CaseTimeline.Create(
            caseEntity.Id,
            "تم فتح القضية",
            $"تم فتح القضية رقم {request.InternalNumber} بعنوان '{request.Title}'",
            true,
            request.CreatedBy);

        if (timelineResult.IsSuccess)
        {
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(caseEntity.Id);
    }
}
