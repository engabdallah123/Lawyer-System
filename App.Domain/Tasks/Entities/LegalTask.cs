using App.Domain.Common;
using App.Domain.Tasks.Enums;
using Shared.Domain;

namespace App.Domain.Tasks.Entities;

/// <summary>
/// المهمة — يمكن ربطها بقضية
/// </summary>
public sealed class LegalTask : Entity, IAuditable
{
    public Guid? CaseId { get; private set; }
    public string AssignedToUserId { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public TaskPriority Priority { get; private set; }
    public LegalTaskStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Cases.Entities.Case? Case { get; private set; }

    private LegalTask() { }

    private LegalTask(
        Guid id, Guid? caseId, string assignedToUserId, string title,
        string? description, DateTime? dueDate, TaskPriority priority)
        : base(id)
    {
        CaseId = caseId;
        AssignedToUserId = assignedToUserId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        Status = LegalTaskStatus.Pending;
    }

    public static Result<LegalTask> Create(
        Guid? caseId, string assignedToUserId, string title,
        string? description, DateTime? dueDate, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<LegalTask>.Failure(Errors.TaskErrors.TitleRequired);

        if (string.IsNullOrWhiteSpace(assignedToUserId))
            return Result<LegalTask>.Failure(Errors.TaskErrors.AssignedUserRequired);

        var task = new LegalTask(
            Guid.NewGuid(), caseId, assignedToUserId,
            title.Trim(), description?.Trim(), dueDate, priority);

        return Result<LegalTask>.Success(task);
    }

    public Result Update(string title, string? description, DateTime? dueDate, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Errors.TaskErrors.TitleRequired);

        Title = title.Trim();
        Description = description?.Trim();
        DueDate = dueDate;
        Priority = priority;

        return Result.Success();
    }

    public void StartProgress() => Status = LegalTaskStatus.InProgress;

    public void Complete()
    {
        Status = LegalTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel() => Status = LegalTaskStatus.Cancelled;

    // IAuditable
    public void SetCreated(string createdBy) { CreatedAt = DateTime.UtcNow; CreatedBy = createdBy; }
    public void SetUpdated(string updatedBy) { UpdatedAt = DateTime.UtcNow; UpdatedBy = updatedBy; }
}
