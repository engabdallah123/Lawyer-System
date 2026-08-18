namespace App.Application.Cases.DTOs;

public record CaseTimelineDto(
    Guid Id,
    Guid CaseId,
    string Title,
    string? Description,
    bool IsImportant,
    DateTime CreatedAt,
    string CreatedBy);
