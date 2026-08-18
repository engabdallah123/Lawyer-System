using App.Domain.Common;
using Shared.Domain;

namespace App.Domain.Cases.Entities;

/// <summary>
/// القضية — الكيان الأساسي للقضايا القانونية
/// </summary>
public sealed class Case : Entity, ISoftDeletable, IAuditable
{
    public string InternalNumber { get; private set; } = null!;
    public string? CourtNumber { get; private set; }
    public string Title { get; private set; } = null!;
    public int CaseTypeId { get; private set; }
    public int CaseStatusId { get; private set; }
    public int? CourtId { get; private set; }
    public string? Circuit { get; private set; }
    public string? JudgeName { get; private set; }
    public DateTime OpenDate { get; private set; }
    public DateTime? CloseDate { get; private set; }
    public decimal? ClaimAmount { get; private set; }
    public string? Description { get; private set; }
    public string? CurrentStage { get; private set; }
    public string? Notes { get; private set; }

    // ISoftDeletable
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties — Lookups
    public Lookups.CaseType CaseType { get; private set; } = null!;
    public Lookups.CaseStatus CaseStatus { get; private set; } = null!;
    public Lookups.Court? Court { get; private set; }

    // Navigation Properties — Collections
    public ICollection<CaseParty> CaseParties { get; private set; } = new List<CaseParty>();
    public ICollection<CaseAssignment> CaseAssignments { get; private set; } = new List<CaseAssignment>();
    public ICollection<Hearings.Entities.Hearing> Hearings { get; private set; } = new List<Hearings.Entities.Hearing>();
    public ICollection<CaseTimeline> CaseTimelines { get; private set; } = new List<CaseTimeline>();
    public ICollection<Documents.Entities.Document> Documents { get; private set; } = new List<Documents.Entities.Document>();
    public ICollection<Tasks.Entities.LegalTask> Tasks { get; private set; } = new List<Tasks.Entities.LegalTask>();
    public ICollection<Finance.Entities.FeeAgreement> FeeAgreements { get; private set; } = new List<Finance.Entities.FeeAgreement>();
    public ICollection<Finance.Entities.Payment> Payments { get; private set; } = new List<Finance.Entities.Payment>();
    public ICollection<Finance.Entities.Expense> Expenses { get; private set; } = new List<Finance.Entities.Expense>();
    public ICollection<Finance.Entities.Invoice> Invoices { get; private set; } = new List<Finance.Entities.Invoice>();
    public ICollection<PowerOfAttorney.Entities.PowerOfAttorney> PowerOfAttorneys { get; private set; } = new List<PowerOfAttorney.Entities.PowerOfAttorney>();

    // EF Core Constructor
    private Case() { }

    private Case(
        Guid id,
        string internalNumber,
        string? courtNumber,
        string title,
        int caseTypeId,
        int caseStatusId,
        int? courtId,
        string? circuit,
        string? judgeName,
        DateTime openDate,
        decimal? claimAmount,
        string? description,
        string? currentStage,
        string? notes)
        : base(id)
    {
        InternalNumber = internalNumber;
        CourtNumber = courtNumber;
        Title = title;
        CaseTypeId = caseTypeId;
        CaseStatusId = caseStatusId;
        CourtId = courtId;
        Circuit = circuit;
        JudgeName = judgeName;
        OpenDate = openDate;
        ClaimAmount = claimAmount;
        Description = description;
        CurrentStage = currentStage;
        Notes = notes;
        IsDeleted = false;
    }

    public static Result<Case> Create(
        string internalNumber,
        string? courtNumber,
        string title,
        int caseTypeId,
        int caseStatusId,
        int? courtId,
        string? circuit,
        string? judgeName,
        DateTime openDate,
        decimal? claimAmount,
        string? description,
        string? currentStage,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(internalNumber))
            return Result<Case>.Failure(Errors.CaseErrors.InternalNumberRequired);

        if (string.IsNullOrWhiteSpace(title))
            return Result<Case>.Failure(Errors.CaseErrors.TitleRequired);

        var caseEntity = new Case(
            Guid.NewGuid(),
            internalNumber.Trim(),
            courtNumber?.Trim(),
            title.Trim(),
            caseTypeId,
            caseStatusId,
            courtId,
            circuit?.Trim(),
            judgeName?.Trim(),
            openDate,
            claimAmount,
            description?.Trim(),
            currentStage?.Trim(),
            notes?.Trim());

        return Result<Case>.Success(caseEntity);
    }

    public Result Update(
        string internalNumber,
        string? courtNumber,
        string title,
        int caseTypeId,
        int caseStatusId,
        int? courtId,
        string? circuit,
        string? judgeName,
        decimal? claimAmount,
        string? description,
        string? currentStage,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(internalNumber))
            return Result.Failure(Errors.CaseErrors.InternalNumberRequired);

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Errors.CaseErrors.TitleRequired);

        InternalNumber = internalNumber.Trim();
        CourtNumber = courtNumber?.Trim();
        Title = title.Trim();
        CaseTypeId = caseTypeId;
        CaseStatusId = caseStatusId;
        CourtId = courtId;
        Circuit = circuit?.Trim();
        JudgeName = judgeName?.Trim();
        ClaimAmount = claimAmount;
        Description = description?.Trim();
        CurrentStage = currentStage?.Trim();
        Notes = notes?.Trim();

        return Result.Success();
    }

    public void CloseCase(DateTime closeDate)
    {
        CloseDate = closeDate;
    }

    public void ReopenCase()
    {
        CloseDate = null;
    }

    public void UpdateStage(string stage)
    {
        CurrentStage = stage;
    }

    // ISoftDeletable
    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
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
