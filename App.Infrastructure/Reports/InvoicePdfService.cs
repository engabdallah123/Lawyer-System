using App.Application.Finance.DTOs;
using App.Application.Finance.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace App.Infrastructure.Reports;

public sealed class InvoicePdfService : IInvoicePdfService
{
    static InvoicePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateInvoicePdf(InvoiceDetailsDto invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(x => x.FontFamily("Segoe UI").FontSize(10).FontColor("#1e293b"));

                // Header
                page.Header().Element(header => ComposeHeader(header, invoice));

                // Content
                page.Content().Element(content => ComposeContent(content, invoice));

                // Footer
                page.Footer().Element(footer => ComposeFooter(footer));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, InvoiceDetailsDto invoice)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Right side: Law firm title matching website branding
                row.RelativeItem(3).Column(c =>
                {
                    c.Item().Text("مكتب المستشار / علاء خالد علام").FontSize(17).Bold().FontColor("#1e3a8a");
                    c.Item().Text("للمحاماة والاستشارات القانونية وأعمال التحكيم").FontSize(9).Bold().FontColor("#c5a059");
                    c.Item().Text("Advocate & Legal Consultant — Alaa Khaled Allam").FontSize(8).FontColor("#64748b");
                    c.Item().Text("القاهرة - جمهورية مصر العربية | هاتف: 01000000000").FontSize(8).FontColor("#94a3b8");
                });

                // Left side: Invoice title & Number
                row.RelativeItem(2).AlignLeft().Column(c =>
                {
                    c.Item().Text("فاتورة أتعاب ومطالبة مالية").FontSize(14).Bold().FontColor("#1e3a8a");
                    c.Item().Text($"رقم الفاتورة: #{invoice.InvoiceNumber}").FontSize(10).Bold().FontColor("#c5a059");
                    c.Item().Text($"التاريخ: {invoice.IssueDate:yyyy/MM/dd}").FontSize(9).FontColor("#64748b");
                });
            });

            col.Item().PaddingTop(10).LineHorizontal(1.5f).LineColor("#c5a059");
        });
    }

    private void ComposeContent(IContainer container, InvoiceDetailsDto invoice)
    {
        container.PaddingTop(15).Column(col =>
        {
            // 1. Client & Case Info (Simple 2-line layout)
            col.Item().Background("#f8fafc").Border(1).BorderColor("#e2e8f0").Padding(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(t =>
                    {
                        t.Span("الموكل: ").Bold().FontColor("#0f172a");
                        t.Span(invoice.ClientName).Bold();
                    });

                    if (!string.IsNullOrEmpty(invoice.ClientPhone))
                    {
                        c.Item().PaddingTop(3).Text(t =>
                        {
                            t.Span("الهاتف: ").Bold().FontColor("#64748b");
                            t.Span(invoice.ClientPhone);
                        });
                    }
                });

                row.RelativeItem().Column(c =>
                {
                    if (!string.IsNullOrEmpty(invoice.CaseInternalNumber))
                    {
                        c.Item().Text(t =>
                        {
                            t.Span("القضية / الملف: ").Bold().FontColor("#0f172a");
                            t.Span($"{invoice.CaseInternalNumber} - {invoice.CaseTitle ?? string.Empty}");
                        });
                    }
                    else
                    {
                        c.Item().Text("أتعاب واستشارات قانونية عامة").FontColor("#64748b");
                    }

                    c.Item().PaddingTop(3).Text(t =>
                    {
                        t.Span("حالة الفاتورة: ").Bold().FontColor("#64748b");
                        t.Span(invoice.StatusName).Bold().FontColor(invoice.RemainingAmount <= 0 ? "#16a34a" : "#dc2626");
                    });
                });
            });

            // 2. Services / Fee Items Table
            col.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);   // #
                    columns.RelativeColumn(5);    // Description
                    columns.RelativeColumn(1.5f); // Amount
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background("#0f172a").Padding(6).AlignCenter().Text("#").FontColor("#ffffff").Bold().FontSize(9);
                    header.Cell().Background("#0f172a").Padding(6).Text("بيان الخدمة / الأتعاب القانونية").FontColor("#ffffff").Bold().FontSize(9);
                    header.Cell().Background("#0f172a").Padding(6).AlignRight().Text("المبلغ (ج.م)").FontColor("#ffffff").Bold().FontSize(9);
                });

                if (invoice.Items != null && invoice.Items.Any())
                {
                    int index = 1;
                    foreach (var item in invoice.Items)
                    {
                        var bg = index % 2 == 0 ? "#f8fafc" : "#ffffff";
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#f1f5f9").Padding(6).AlignCenter().Text(index.ToString()).FontSize(9);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#f1f5f9").Padding(6).Text(item.Description).FontSize(9);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#f1f5f9").Padding(6).AlignRight().Text($"{item.Total:N2}").Bold().FontSize(9);
                        index++;
                    }
                }
                else
                {
                    table.Cell().Background("#ffffff").BorderBottom(1).BorderColor("#f1f5f9").Padding(6).AlignCenter().Text("1").FontSize(9);
                    table.Cell().Background("#ffffff").BorderBottom(1).BorderColor("#f1f5f9").Padding(6).Text($"أتعاب ومصروفات قانونية (فاتورة #{invoice.InvoiceNumber})").FontSize(9);
                    table.Cell().Background("#ffffff").BorderBottom(1).BorderColor("#f1f5f9").Padding(6).AlignRight().Text($"{invoice.TotalAmount:N2}").Bold().FontSize(9);
                }
            });

            // 3. Clear Financial Breakdown (Total, Paid, Remaining)
            col.Item().PaddingTop(10).Row(row =>
            {
                // Left Notes (if any)
                row.RelativeItem(3).Column(c =>
                {
                    if (!string.IsNullOrEmpty(invoice.Notes))
                    {
                        c.Item().Text("ملاحظات:").Bold().FontSize(9).FontColor("#64748b");
                        c.Item().PaddingTop(2).Text(invoice.Notes).FontSize(9);
                    }
                });

                // Right: Financial Summary Box
                row.RelativeItem(2).Border(1).BorderColor("#cbd5e1").Background("#f8fafc").Padding(10).Column(c =>
                {
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("إجمالي الأتعاب:").Bold().FontSize(10);
                        r.RelativeItem().AlignLeft().Text($"{invoice.TotalAmount:N2} ج.م").Bold().FontSize(10);
                    });

                    c.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text("المبلغ المسدد:").Bold().FontSize(10).FontColor("#16a34a");
                        r.RelativeItem().AlignLeft().Text($"{invoice.PaidAmount:N2} ج.م").Bold().FontSize(10).FontColor("#16a34a");
                    });

                    c.Item().PaddingVertical(4).LineHorizontal(1).LineColor("#cbd5e1");

                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("المتبقي عليك:").Bold().FontSize(11).FontColor(invoice.RemainingAmount > 0 ? "#dc2626" : "#16a34a");
                        r.RelativeItem().AlignLeft().Text($"{invoice.RemainingAmount:N2} ج.م").Bold().FontSize(12).FontColor(invoice.RemainingAmount > 0 ? "#dc2626" : "#16a34a");
                    });
                });
            });

            // 4. Payment Receipts Summary (if any payments exist)
            if (invoice.Payments != null && invoice.Payments.Any())
            {
                col.Item().PaddingTop(15).Text("سجل الدفعات المستلمة:").Bold().FontSize(10).FontColor("#0f172a");

                col.Item().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Date
                        columns.RelativeColumn(2); // Amount
                        columns.RelativeColumn(2); // Method
                        columns.RelativeColumn(2); // Received By
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#e2e8f0").Padding(4).Text("تاريخ الدفعة").Bold().FontSize(8);
                        header.Cell().Background("#e2e8f0").Padding(4).AlignRight().Text("المبلغ المستلم").Bold().FontSize(8);
                        header.Cell().Background("#e2e8f0").Padding(4).Text("طريقة السداد").Bold().FontSize(8);
                        header.Cell().Background("#e2e8f0").Padding(4).Text("المستلم").Bold().FontSize(8);
                    });

                    foreach (var p in invoice.Payments)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#f1f5f9").Padding(4).Text(p.PaymentDate.ToString("yyyy/MM/dd")).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor("#f1f5f9").Padding(4).AlignRight().Text($"{p.Amount:N2} ج.م").Bold().FontSize(8).FontColor("#16a34a");
                        table.Cell().BorderBottom(1).BorderColor("#f1f5f9").Padding(4).Text(p.PaymentMethod).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor("#f1f5f9").Padding(4).Text(p.ReceivedBy).FontSize(8);
                    }
                });
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            // Simple Signatures & Stamp Row
            col.Item().PaddingTop(10).Row(row =>
            {
                // Client Signature
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("توقيع واستلام الموكل:").Bold().FontSize(9).FontColor("#0f172a");
                    c.Item().PaddingTop(15).Text(".............................................").FontSize(9);
                });

                // Office Stamp & Signature
                row.RelativeItem().AlignLeft().Column(c =>
                {
                    c.Item().Text("ختم واعتماد مكتب المستشار / علاء خالد علام:").Bold().FontSize(9).FontColor("#1e3a8a");
                    c.Item().PaddingTop(15).Text(".............................................").FontSize(9);
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(0.5f).LineColor("#e2e8f0");

            // Timestamp
            col.Item().PaddingTop(2).Row(row =>
            {
                row.RelativeItem().Text($"حررت بتاريخ: {DateTime.Now:yyyy/MM/dd hh:mm tt}").FontSize(7).FontColor("#94a3b8");
                row.RelativeItem().AlignLeft().Text(x =>
                {
                    x.Span("صفحة ");
                    x.CurrentPageNumber();
                    x.Span(" من ");
                    x.TotalPages();
                });
            });
        });
    }
}
