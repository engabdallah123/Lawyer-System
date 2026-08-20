using App.Application.Finance.DTOs;

namespace App.Application.Finance.Services;

/// <summary>
/// خدمة توليد وتصدير فواتير الأتعاب بصيغة PDF عبر QuestPDF
/// </summary>
public interface IInvoicePdfService
{
    byte[] GenerateInvoicePdf(InvoiceDetailsDto invoice);
}
