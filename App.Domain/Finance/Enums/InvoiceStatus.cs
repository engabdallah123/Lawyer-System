namespace App.Domain.Finance.Enums;

/// <summary>
/// حالة الفاتورة
/// </summary>
public enum InvoiceStatus
{
    /// <summary>مسودة</summary>
    Draft = 0,

    /// <summary>صادرة</summary>
    Issued = 1,

    /// <summary>مدفوعة جزئيًا</summary>
    PartiallyPaid = 2,

    /// <summary>مدفوعة بالكامل</summary>
    Paid = 3,

    /// <summary>ملغاة</summary>
    Cancelled = 4,

    /// <summary>متأخرة</summary>
    Overdue = 5
}
