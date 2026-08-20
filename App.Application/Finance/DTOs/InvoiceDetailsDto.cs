using App.Domain.Finance.Enums;

namespace App.Application.Finance.DTOs;

public class InvoiceDetailsDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientPhone { get; set; }
    public string? ClientAddress { get; set; }
    public Guid? CaseId { get; set; }
    public string? CaseInternalNumber { get; set; }
    public string? CaseTitle { get; set; }
    public Guid? FeeAgreementId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? QRCodePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    public IEnumerable<InvoiceItemDto> Items { get; set; } = [];
    public IEnumerable<PaymentDto> Payments { get; set; } = [];
}
