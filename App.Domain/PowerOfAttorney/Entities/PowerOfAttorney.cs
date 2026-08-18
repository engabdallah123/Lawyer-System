using App.Domain.Common;
using Shared.Domain;

namespace App.Domain.PowerOfAttorney.Entities;

/// <summary>
/// التوكيل — مرتبط بموكل ويمكن ربطه بقضية
/// </summary>
public sealed class PowerOfAttorney : Entity, IAuditable
{
    public Guid ClientId { get; private set; }
    public Guid? CaseId { get; private set; }
    public string PowerNumber { get; private set; } = null!;
    public DateTime IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? NotaryName { get; private set; }
    public string? NotaryNumber { get; private set; }
    public string? FilePath { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Clients.Entities.Client Client { get; private set; } = null!;
    public Cases.Entities.Case? Case { get; private set; }

    // EF Core Constructor
    private PowerOfAttorney() { }

    private PowerOfAttorney(
        Guid id,
        Guid clientId,
        Guid? caseId,
        string powerNumber,
        DateTime issueDate,
        DateTime? expiryDate,
        string? notaryName,
        string? notaryNumber,
        string? filePath,
        string? notes)
        : base(id)
    {
        ClientId = clientId;
        CaseId = caseId;
        PowerNumber = powerNumber;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        NotaryName = notaryName;
        NotaryNumber = notaryNumber;
        FilePath = filePath;
        Notes = notes;
        IsActive = true;
    }

    public static Result<PowerOfAttorney> Create(
        Guid clientId,
        Guid? caseId,
        string powerNumber,
        DateTime issueDate,
        DateTime? expiryDate,
        string? notaryName,
        string? notaryNumber,
        string? filePath,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(powerNumber))
            return Result<PowerOfAttorney>.Failure(Errors.PowerOfAttorneyErrors.PowerNumberRequired);

        var poa = new PowerOfAttorney(
            Guid.NewGuid(),
            clientId,
            caseId,
            powerNumber.Trim(),
            issueDate,
            expiryDate,
            notaryName?.Trim(),
            notaryNumber?.Trim(),
            filePath,
            notes?.Trim());

        return Result<PowerOfAttorney>.Success(poa);
    }

    public Result Update(
        Guid? caseId,
        string powerNumber,
        DateTime issueDate,
        DateTime? expiryDate,
        string? notaryName,
        string? notaryNumber,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(powerNumber))
            return Result.Failure(Errors.PowerOfAttorneyErrors.PowerNumberRequired);

        CaseId = caseId;
        PowerNumber = powerNumber.Trim();
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        NotaryName = notaryName?.Trim();
        NotaryNumber = notaryNumber?.Trim();
        Notes = notes?.Trim();

        return Result.Success();
    }

    public void SetFilePath(string filePath) => FilePath = filePath;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

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
