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
        // 1. إنشاء كيان القضية الأساسي
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

        // 2. إضافة الموكل الرئيسي كطرف في القضية مع صفته المحددة
        var clientPartyResult = CaseParty.Create(
            caseEntity.Id,
            request.ClientId,
            null,
            null,
            request.ClientRole,
            true,
            null,
            null,
            null,
            null,
            null,
            "الموكل الأساسي صاحب الملف");

        if (clientPartyResult.IsFailure)
            return Result<Guid>.Failure(clientPartyResult.Error);

        await _unitOfWork.CaseParties.AddAsync(clientPartyResult.Value!, cancellationToken);

        // 3. إضافة الخصوم والأطراف الإضافية إن وجدت
        if (request.AdditionalParties != null && request.AdditionalParties.Count > 0)
        {
            foreach (var partyDto in request.AdditionalParties)
            {
                if (string.IsNullOrWhiteSpace(partyDto.PartyName) && !partyDto.ClientId.HasValue)
                    continue;

                var partyResult = CaseParty.Create(
                    caseEntity.Id,
                    partyDto.ClientId,
                    partyDto.PartyName,
                    partyDto.PartyType,
                    partyDto.PartyRole,
                    false,
                    partyDto.Address,
                    partyDto.Phone,
                    partyDto.Email,
                    partyDto.LawyerName,
                    partyDto.LawyerPhone,
                    partyDto.Notes);

                if (partyResult.IsSuccess)
                {
                    await _unitOfWork.CaseParties.AddAsync(partyResult.Value!, cancellationToken);
                }
            }
        }

        // 4. إضافة المحامي الرئيسي إذا حُدد
        if (!string.IsNullOrWhiteSpace(request.MainLawyerUserId))
        {
            var assignmentResult = CaseAssignment.Create(
                caseEntity.Id,
                request.MainLawyerUserId,
                "محامي أساسي",
                DateTime.UtcNow,
                "تم التعيين عند فتح ملف القضية");

            if (assignmentResult.IsSuccess)
            {
                await _unitOfWork.CaseAssignments.AddAsync(assignmentResult.Value!, cancellationToken);
            }
        }

        // 5. إضافة مدخل Timeline تلقائي
        var timelineResult = CaseTimeline.Create(
            caseEntity.Id,
            "تم فتح ملف القضية",
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
