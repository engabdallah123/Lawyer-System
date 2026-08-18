namespace App.Application.Finance.DTOs;

public record InvoiceItemDto(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total);
