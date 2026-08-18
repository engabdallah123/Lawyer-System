namespace App.Application.Lookups.DTOs;

public record LookupDto(
    int Id,
    string Name,
    string? Description = null,
    string? Extra = null);
