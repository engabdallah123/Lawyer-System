namespace App.Application.Cases.DTOs;

public class CaseDetailsDto
{
    public Guid Id { get; set; }
    public string InternalNumber { get; set; } = string.Empty;
    public string? CourtNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CaseTypeId { get; set; }
    public string CaseTypeName { get; set; } = string.Empty;
    public int CaseStatusId { get; set; }
    public string CaseStatusName { get; set; } = string.Empty;
    public string? CaseStatusColor { get; set; }
    public int? CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? Circuit { get; set; }
    public string? JudgeName { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public decimal? ClaimAmount { get; set; }
    public string? Description { get; set; }
    public string? CurrentStage { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public int HearingsCount { get; set; }
    public int DocumentsCount { get; set; }
    public int TasksCount { get; set; }
    public decimal TotalFees { get; set; }
    public decimal PaidFees { get; set; }
    public decimal RemainingFees { get; set; }

    public IEnumerable<CasePartyDto> Parties { get; set; } = [];
    public IEnumerable<CaseAssignmentDto> Assignments { get; set; } = [];
    public IEnumerable<CaseTimelineDto> Timelines { get; set; } = [];
}
