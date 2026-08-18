using App.Application.Cases.DTOs;
using App.Application.Hearings.DTOs;
using App.Application.Tasks.DTOs;

namespace App.Application.Dashboard.DTOs;

public record LawyerDashboardDto(
    IEnumerable<HearingDto> TodayHearings,
    IEnumerable<HearingDto> WeekHearings,
    IEnumerable<CaseDto> AssignedCases,
    IEnumerable<LegalTaskDto> OverdueTasks,
    IEnumerable<LegalTaskDto> UpcomingTasks,
    IEnumerable<CaseTimelineDto> RecentUpdates,
    int UnreadNotificationsCount);

public record CaseTypeStatDto(
    string TypeName,
    int CasesCount);

public record LawyerPerformanceStatDto(
    string UserId,
    int ActiveCasesCount,
    int ClosedCasesCount,
    int TotalHearingsCount,
    int CompletedTasksCount,
    int PendingTasksCount);

public record OwnerDashboardDto(
    int TotalCasesCount,
    int OpenCasesCount,
    int ClosedCasesCount,
    int TodayHearingsCount,
    int WeekHearingsCount,
    decimal TotalAgreedFees,
    decimal TotalCollectedFees,
    decimal TotalOutstandingReceivables,
    decimal TotalExpenses,
    decimal NetRevenue,
    int TotalActiveClientsCount,
    int ExpiringPoasCount,
    IEnumerable<CaseTypeStatDto> CasesByType,
    IEnumerable<LawyerPerformanceStatDto> LawyerPerformance,
    IEnumerable<HearingDto> TodayHearings,
    IEnumerable<CaseTimelineDto> RecentOfficeUpdates);
