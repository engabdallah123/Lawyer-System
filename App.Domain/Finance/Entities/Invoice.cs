using App.Domain.Common;
using App.Domain.Finance.Enums;
using Shared.Domain;

namespace App.Domain.Finance.Entities;

/// <summary>
/// الفاتورة — مرتبطة بموكل ويمكن ربطها بقضية وعقد أتعاب
/// </summary>
public sealed class Invoice : Entity, IAuditable
{
    public string InvoiceNumber { get; private set; } = null!;
    public Guid ClientId { get; private set; }
    public Guid? CaseId { get; private set; }
    public Guid? FeeAgreementId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? QRCodePath { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation Properties
    public Clients.Entities.Client Client { get; private set; } = null!;
    public Cases.Entities.Case? Case { get; private set; }
    public FeeAgreement? FeeAgreement { get; private set; }
    public ICollection<InvoiceItem> InvoiceItems { get; private set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    private Invoice() { }

    private Invoice(
        Guid id, string invoiceNumber, Guid clientId, Guid? caseId, Guid? feeAgreementId,
        DateTime issueDate, DateTime? dueDate, decimal subTotal, decimal discount,
        decimal taxAmount, decimal totalAmount, string? notes)
        : base(id)
    {
        InvoiceNumber = invoiceNumber;
        ClientId = clientId;
        CaseId = caseId;
        FeeAgreementId = feeAgreementId;
        IssueDate = issueDate;
        DueDate = dueDate;
        SubTotal = subTotal;
        Discount = discount;
        TaxAmount = taxAmount;
        TotalAmount = totalAmount;
        PaidAmount = 0;
        Status = InvoiceStatus.Draft;
        Notes = notes;
    }

    public static Result<Invoice> Create(
        string invoiceNumber, Guid clientId, Guid? caseId, Guid? feeAgreementId,
        DateTime issueDate, DateTime? dueDate, decimal subTotal, decimal discount,
        decimal taxAmount, string? notes)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            return Result<Invoice>.Failure(Errors.FinanceErrors.InvoiceNumberRequired);

        var totalAmount = subTotal - discount + taxAmount;

        var invoice = new Invoice(
            Guid.NewGuid(), invoiceNumber.Trim(), clientId, caseId, feeAgreementId,
            issueDate, dueDate, subTotal, discount, taxAmount, totalAmount, notes?.Trim());

        return Result<Invoice>.Success(invoice);
    }

    public void Issue() => Status = InvoiceStatus.Issued;
    public void Cancel() => Status = InvoiceStatus.Cancelled;
    public void MarkAsOverdue() => Status = InvoiceStatus.Overdue;

    /// <summary>
    /// تحديث المبلغ المدفوع وحالة الفاتورة
    /// </summary>
    public void AddPaymentAmount(decimal amount)
    {
        PaidAmount += amount;

        if (PaidAmount >= TotalAmount)
            Status = InvoiceStatus.Paid;
        else if (PaidAmount > 0)
            Status = InvoiceStatus.PartiallyPaid;
    }

    public void RecalculateTotal()
    {
        SubTotal = 0;
        foreach (var item in InvoiceItems)
            SubTotal += item.Total;

        TotalAmount = SubTotal - Discount + TaxAmount;
    }

    public void SetQRCodePath(string path) => QRCodePath = path;

    public decimal RemainingAmount => TotalAmount - PaidAmount;

    // IAuditable
    public void SetCreated(string createdBy) { CreatedAt = DateTime.UtcNow; CreatedBy = createdBy; }
    public void SetUpdated(string updatedBy) { UpdatedAt = DateTime.UtcNow; UpdatedBy = updatedBy; }
}
