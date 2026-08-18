using App.Domain.Common;
using Shared.Domain;

namespace App.Domain.Hearings.Entities;

/// <summary>
/// الجلسة — مرتبطة دائمًا بقضية
/// </summary>
public sealed class Hearing : Entity, IAuditable
{
    public Guid CaseId { get; private set; }
    public DateTime HearingDate { get; private set; }
    public TimeSpan? HearingTime { get; private set; }
    public string HearingType { get; private set; } = null!;
    public string? Result { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? NextHearingDate { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Cases.Entities.Case Case { get; private set; } = null!;

    // EF Core Constructor
    private Hearing() { }

    private Hearing(
        Guid id,
        Guid caseId,
        DateTime hearingDate,
        TimeSpan? hearingTime,
        string hearingType,
        string? notes)
        : base(id)
    {
        CaseId = caseId;
        HearingDate = hearingDate;
        HearingTime = hearingTime;
        HearingType = hearingType;
        Notes = notes;
    }

    public static Result<Hearing> Create(
        Guid caseId,
        DateTime hearingDate,
        TimeSpan? hearingTime,
        string hearingType,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(hearingType))
            return Result<Hearing>.Failure(Errors.HearingErrors.HearingTypeRequired);

        var hearing = new Hearing(
            Guid.NewGuid(),
            caseId,
            hearingDate,
            hearingTime,
            hearingType.Trim(),
            notes?.Trim());

        return Result<Hearing>.Success(hearing);
    }

    /// <summary>
    /// تسجيل نتيجة الجلسة وتحديد الجلسة القادمة
    /// </summary>
    public Result RecordResult(string? result, string? notes, DateTime? nextHearingDate)
    {
        Result = result?.Trim();
        Notes = notes?.Trim();
        NextHearingDate = nextHearingDate;

        return Shared.Domain.Result.Success();
    }

    /// <summary>
    /// إعادة جدولة الجلسة
    /// </summary>
    public Result Reschedule(DateTime newDate, TimeSpan? newTime)
    {
        HearingDate = newDate;
        HearingTime = newTime;

        return Shared.Domain.Result.Success();
    }

    // IAuditable
    public void SetCreated(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
