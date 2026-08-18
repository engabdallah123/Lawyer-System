using App.Domain.Common;
using App.Domain.Finance.Enums;
using Shared.Domain;

namespace App.Domain.Finance.Entities;

/// <summary>
/// عقد الأتعاب — مرتبط بموكل ويمكن ربطه بقضية
/// </summary>
public sealed class FeeAgreement : Entity, IAuditable
{
    public Guid? CaseId { get; private set; }
    public Guid ClientId { get; private set; }
    public AgreementType AgreementType { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Clients.Entities.Client Client { get; private set; } = null!;
    public Cases.Entities.Case? Case { get; private set; }
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    private FeeAgreement() { }

    private FeeAgreement(
        Guid id, Guid clientId, Guid? caseId, AgreementType agreementType,
        decimal totalAmount, string? description, DateTime startDate, DateTime? endDate)
        : base(id)
    {
        ClientId = clientId;
        CaseId = caseId;
        AgreementType = agreementType;
        TotalAmount = totalAmount;
        PaidAmount = 0;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<FeeAgreement> Create(
        Guid clientId, Guid? caseId, AgreementType agreementType,
        decimal totalAmount, string? description, DateTime startDate, DateTime? endDate)
    {
        if (totalAmount <= 0)
            return Result<FeeAgreement>.Failure(Errors.FinanceErrors.InvalidAmount);

        var agreement = new FeeAgreement(
            Guid.NewGuid(), clientId, caseId, agreementType,
            totalAmount, description?.Trim(), startDate, endDate);

        return Result<FeeAgreement>.Success(agreement);
    }

    public Result Update(
        AgreementType agreementType, decimal totalAmount,
        string? description, DateTime startDate, DateTime? endDate)
    {
        if (totalAmount <= 0)
            return Result.Failure(Errors.FinanceErrors.InvalidAmount);

        AgreementType = agreementType;
        TotalAmount = totalAmount;
        Description = description?.Trim();
        StartDate = startDate;
        EndDate = endDate;

        return Result.Success();
    }

    /// <summary>
    /// تحديث المبلغ المدفوع عند تسجيل دفعة
    /// </summary>
    public void AddPaymentAmount(decimal amount) => PaidAmount += amount;

    /// <summary>
    /// المبلغ المتبقي
    /// </summary>
    public decimal RemainingAmount => TotalAmount - PaidAmount;

    // IAuditable
    public void SetCreated(string createdBy) { CreatedAt = DateTime.UtcNow; CreatedBy = createdBy; }
    public void SetUpdated(string updatedBy) { UpdatedAt = DateTime.UtcNow; UpdatedBy = updatedBy; }
}
