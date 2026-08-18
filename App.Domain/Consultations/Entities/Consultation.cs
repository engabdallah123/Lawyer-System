using App.Domain.Common;
using App.Domain.Consultations.Enums;
using Shared.Domain;

namespace App.Domain.Consultations.Entities;

/// <summary>
/// الاستشارة — مرتبطة بموكل وليست شرطًا لفتح قضية
/// </summary>
public sealed class Consultation : Entity, IAuditable
{
    public Guid ClientId { get; private set; }
    public DateTime ConsultationDate { get; private set; }
    public string Subject { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal? Fee { get; private set; }
    public ConsultationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Clients.Entities.Client Client { get; private set; } = null!;

    // EF Core Constructor
    private Consultation() { }

    private Consultation(
        Guid id,
        Guid clientId,
        DateTime consultationDate,
        string subject,
        string? description,
        decimal? fee,
        string? notes)
        : base(id)
    {
        ClientId = clientId;
        ConsultationDate = consultationDate;
        Subject = subject;
        Description = description;
        Fee = fee;
        Status = ConsultationStatus.Scheduled;
        Notes = notes;
    }

    public static Result<Consultation> Create(
        Guid clientId,
        DateTime consultationDate,
        string subject,
        string? description,
        decimal? fee,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return Result<Consultation>.Failure(Errors.ConsultationErrors.SubjectRequired);

        var consultation = new Consultation(
            Guid.NewGuid(),
            clientId,
            consultationDate,
            subject.Trim(),
            description?.Trim(),
            fee,
            notes?.Trim());

        return Result<Consultation>.Success(consultation);
    }

    public Result Update(
        DateTime consultationDate,
        string subject,
        string? description,
        decimal? fee,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return Result.Failure(Errors.ConsultationErrors.SubjectRequired);

        ConsultationDate = consultationDate;
        Subject = subject.Trim();
        Description = description?.Trim();
        Fee = fee;
        Notes = notes?.Trim();

        return Result.Success();
    }

    public void Complete() => Status = ConsultationStatus.Completed;
    public void Cancel() => Status = ConsultationStatus.Cancelled;

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
