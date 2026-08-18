using App.Domain.Tasks.Enums;

namespace App.Application.Tasks.DTOs;

public record LegalTaskDto(
    Guid Id,
    Guid? CaseId,
    string? CaseInternalNumber,
    string? CaseTitle,
    string AssignedToUserId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskPriority Priority,
    string PriorityName,
    LegalTaskStatus Status,
    string StatusName,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    string CreatedBy);
