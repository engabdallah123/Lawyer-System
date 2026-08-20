using App.Domain;
using App.Domain.Cases.Entities;
using App.Domain.Cases.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Commands.AssignLawyer;

internal sealed class AssignLawyerCommandHandler : ICommandHandler<AssignLawyerCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public AssignLawyerCommandHandler(IAppUnitOfWork unitOfWork, ISqlConnectionFactory sqlConnectionFactory)
    {
        _unitOfWork = unitOfWork;
        _sqlConnectionFactory = sqlConnectionFactory;
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

        // Fetch lawyer's full name
        string lawyerName = request.UserId;
        try
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var name = await connection.ExecuteScalarAsync<string>(
                "SELECT ISNULL(NULLIF(FullName, ''), UserName) FROM AspNetUsers WHERE Id = @UserId",
                new { request.UserId });
            if (!string.IsNullOrWhiteSpace(name))
            {
                lawyerName = name;
            }
        }
        catch { }

        var timelineResult = CaseTimeline.Create(
            request.CaseId,
            "إسناد محامٍ للملف",
            $"تم إسناد القضية إلى المحامي '{lawyerName}' بدور '{request.RoleInCase}'",
            false,
            request.AssignedBy);

        if (timelineResult.IsSuccess)
            await _unitOfWork.CaseTimelines.AddAsync(timelineResult.Value!, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(assignmentResult.Value!.Id);
    }
}
