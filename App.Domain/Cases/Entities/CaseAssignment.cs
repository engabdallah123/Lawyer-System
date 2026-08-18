using Shared.Domain;

namespace App.Domain.Cases.Entities;

/// <summary>
/// توزيع القضية على المحامين
/// </summary>
public sealed class CaseAssignment : Entity
{
    public Guid CaseId { get; private set; }
    public string UserId { get; private set; } = null!;
    public string RoleInCase { get; private set; } = null!;
    public DateTime AssignedDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Case Case { get; private set; } = null!;

    // EF Core Constructor
    private CaseAssignment() { }

    private CaseAssignment(
        Guid id,
        Guid caseId,
        string userId,
        string roleInCase,
        DateTime assignedDate,
        string? notes)
        : base(id)
    {
        CaseId = caseId;
        UserId = userId;
        RoleInCase = roleInCase;
        AssignedDate = assignedDate;
        Notes = notes;
    }

    public static Result<CaseAssignment> Create(
        Guid caseId,
        string userId,
        string roleInCase,
        DateTime assignedDate,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result<CaseAssignment>.Failure(Errors.CaseErrors.UserIdRequired);

        if (string.IsNullOrWhiteSpace(roleInCase))
            return Result<CaseAssignment>.Failure(Errors.CaseErrors.RoleInCaseRequired);

        var assignment = new CaseAssignment(
            Guid.NewGuid(),
            caseId,
            userId,
            roleInCase.Trim(),
            assignedDate,
            notes?.Trim());

        return Result<CaseAssignment>.Success(assignment);
    }

    public Result Update(string roleInCase, string? notes)
    {
        if (string.IsNullOrWhiteSpace(roleInCase))
            return Result.Failure(Errors.CaseErrors.RoleInCaseRequired);

        RoleInCase = roleInCase.Trim();
        Notes = notes?.Trim();

        return Result.Success();
    }
}
