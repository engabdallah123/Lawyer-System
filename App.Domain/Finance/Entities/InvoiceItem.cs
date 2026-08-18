using Shared.Domain;

namespace App.Domain.Finance.Entities;

/// <summary>
/// بند الفاتورة
/// </summary>
public sealed class InvoiceItem : Entity
{
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total { get; private set; }

    // Navigation Properties
    public Invoice Invoice { get; private set; } = null!;

    private InvoiceItem() { }

    private InvoiceItem(Guid id, Guid invoiceId, string description, decimal quantity, decimal unitPrice)
        : base(id)
    {
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Total = quantity * unitPrice;
    }

    public static Result<InvoiceItem> Create(Guid invoiceId, string description, decimal quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result<InvoiceItem>.Failure(Errors.FinanceErrors.InvoiceItemDescriptionRequired);

        if (quantity <= 0 || unitPrice < 0)
            return Result<InvoiceItem>.Failure(Errors.FinanceErrors.InvalidAmount);

        var item = new InvoiceItem(Guid.NewGuid(), invoiceId, description.Trim(), quantity, unitPrice);
        return Result<InvoiceItem>.Success(item);
    }

    public Result Update(string description, decimal quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(Errors.FinanceErrors.InvoiceItemDescriptionRequired);

        Description = description.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        Total = quantity * unitPrice;

        return Result.Success();
    }
}
