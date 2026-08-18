using Shared.Domain;

namespace App.Domain.Finance.Errors;

public static class FinanceErrors
{
    public static readonly Error InvalidAmount =
        new("Finance.InvalidAmount", "المبلغ يجب أن يكون أكبر من صفر.");

    public static readonly Error PaymentMethodRequired =
        new("Payment.MethodRequired", "طريقة الدفع مطلوبة.");

    public static readonly Error ExpenseTypeRequired =
        new("Expense.TypeRequired", "نوع المصروف مطلوب.");

    public static readonly Error InvoiceNumberRequired =
        new("Invoice.NumberRequired", "رقم الفاتورة مطلوب.");

    public static readonly Error InvoiceItemDescriptionRequired =
        new("InvoiceItem.DescriptionRequired", "وصف بند الفاتورة مطلوب.");

    public static Error InvoiceNotFound(Guid id) =>
        Error.NotFound("Invoice.NotFound", $"الفاتورة ذات المعرف '{id}' غير موجودة.");

    public static Error PaymentNotFound(Guid id) =>
        Error.NotFound("Payment.NotFound", $"الدفعة ذات المعرف '{id}' غير موجودة.");

    public static Error FeeAgreementNotFound(Guid id) =>
        Error.NotFound("FeeAgreement.NotFound", $"عقد الأتعاب ذو المعرف '{id}' غير موجود.");
}
