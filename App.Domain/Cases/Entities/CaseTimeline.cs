using Shared.Domain;

namespace App.Domain.Cases.Entities;

/// <summary>
/// سجل تطورات القضية — Timeline
/// </summary>
public sealed class CaseTimeline : Entity
{
    public Guid CaseId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsImportant { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;

    // Navigation Properties
    public Case Case { get; private set; } = null!;

    // EF Core Constructor
    private CaseTimeline() { }

    private CaseTimeline(
        Guid id,
        Guid caseId,
        string title,
        string? description,
        bool isImportant)
        : base(id)
    {
        CaseId = caseId;
        Title = title;
        Description = description;
        IsImportant = isImportant;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<CaseTimeline> Create(
        Guid caseId,
        string title,
        string? description,
        bool isImportant,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<CaseTimeline>.Failure(Errors.CaseErrors.TimelineTitleRequired);

        var timeline = new CaseTimeline(
            Guid.NewGuid(),
            caseId,
            title.Trim(),
            description?.Trim(),
            isImportant);

        timeline.CreatedBy = createdBy;

        return Result<CaseTimeline>.Success(timeline);
    }
}
