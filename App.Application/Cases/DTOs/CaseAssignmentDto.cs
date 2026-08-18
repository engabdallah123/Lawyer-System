namespace App.Application.Cases.DTOs;

public record CaseAssignmentDto(
    Guid Id,
    Guid CaseId,
    string UserId,
    string RoleInCase,
    DateTime AssignedDate,
    string? Notes);
