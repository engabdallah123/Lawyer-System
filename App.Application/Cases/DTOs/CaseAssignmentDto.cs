namespace App.Application.Cases.DTOs;

public class CaseAssignmentDto
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? LawyerName => !string.IsNullOrWhiteSpace(FullName) ? FullName : (!string.IsNullOrWhiteSpace(UserName) ? UserName : UserId);
    public string RoleInCase { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public string? Notes { get; set; }
}
