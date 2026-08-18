using Shared.Domain;

namespace App.Domain.Finance.Entities;

/// <summary>
/// الدفعة — مرتبطة بموكل ويمكن ربطها بقضية وعقد أتعاب وفاتورة
/// </summary>
public sealed class Payment : Entity
{
    public Guid ClientId { get; private set; }
    public Guid? CaseId { get; private set; }
    public Guid? FeeAgreementId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string PaymentMethod { get; private set; } = null!;
    public string? ReferenceNumber { get; private set; }
    public string? Notes { get; private set; }
    public string ReceivedBy { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Navigation Properties
    public Clients.Entities.Client Client { get; private set; } = null!;
    public Cases.Entities.Case? Case { get; private set; }
    public FeeAgreement? FeeAgreement { get; private set; }
    public Invoice? Invoice { get; private set; }

    private Payment() { }

    private Payment(
        Guid id, Guid clientId, Guid? caseId, Guid? feeAgreementId, Guid? invoiceId,
        decimal amount, DateTime paymentDate, string paymentMethod,
        string? referenceNumber, string? notes, string receivedBy)
        : base(id)
    {
        ClientId = clientId;
        CaseId = caseId;
        FeeAgreementId = feeAgreementId;
        InvoiceId = invoiceId;
        Amount = amount;
        PaymentDate = paymentDate;
        PaymentMethod = paymentMethod;
        ReferenceNumber = referenceNumber;
        Notes = notes;
        ReceivedBy = receivedBy;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Payment> Create(
        Guid clientId, Guid? caseId, Guid? feeAgreementId, Guid? invoiceId,
        decimal amount, DateTime paymentDate, string paymentMethod,
        string? referenceNumber, string? notes, string receivedBy)
    {
        if (amount <= 0)
            return Result<Payment>.Failure(Errors.FinanceErrors.InvalidAmount);

        if (string.IsNullOrWhiteSpace(paymentMethod))
            return Result<Payment>.Failure(Errors.FinanceErrors.PaymentMethodRequired);

        var payment = new Payment(
            Guid.NewGuid(), clientId, caseId, feeAgreementId, invoiceId,
            amount, paymentDate, paymentMethod.Trim(),
            referenceNumber?.Trim(), notes?.Trim(), receivedBy);

        return Result<Payment>.Success(payment);
    }
}
