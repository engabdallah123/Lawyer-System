using App.Application.Cases.DTOs;
using App.Domain.Cases.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Cases.Queries.GetCaseById;

internal sealed class GetCaseByIdQueryHandler : IQueryHandler<GetCaseByIdQuery, CaseDetailsDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCaseByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<CaseDetailsDto>> Handle(GetCaseByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var caseSql = @"
            SELECT 
                c.Id,
                c.InternalNumber,
                c.CourtNumber,
                c.Title,
                c.CaseTypeId,
                ct.Name AS CaseTypeName,
                c.CaseStatusId,
                cs.Name AS CaseStatusName,
                cs.Color AS CaseStatusColor,
                c.CourtId,
                crt.Name AS CourtName,
                c.Circuit,
                c.JudgeName,
                c.OpenDate,
                c.CloseDate,
                c.ClaimAmount,
                c.Description,
                c.CurrentStage,
                c.Notes,
                c.CreatedAt,
                c.CreatedBy,
                c.UpdatedAt,
                c.UpdatedBy,
                (SELECT COUNT(*) FROM Hearings h WHERE h.CaseId = c.Id) AS HearingsCount,
                (SELECT COUNT(*) FROM Documents d WHERE d.CaseId = c.Id AND d.IsDeleted = 0) AS DocumentsCount,
                (SELECT COUNT(*) FROM LegalTasks t WHERE t.CaseId = c.Id) AS TasksCount,
                ISNULL((SELECT SUM(TotalAmount) FROM FeeAgreements fa WHERE fa.CaseId = c.Id), 0) AS TotalFees,
                ISNULL((SELECT SUM(PaidAmount) FROM FeeAgreements fa WHERE fa.CaseId = c.Id), 0) AS PaidFees,
                ISNULL((SELECT SUM(TotalAmount - PaidAmount) FROM FeeAgreements fa WHERE fa.CaseId = c.Id), 0) AS RemainingFees
            FROM Cases c
            INNER JOIN CaseTypes ct ON c.CaseTypeId = ct.Id
            INNER JOIN CaseStatuses cs ON c.CaseStatusId = cs.Id
            LEFT JOIN Courts crt ON c.CourtId = crt.Id
            WHERE c.Id = @Id AND c.IsDeleted = 0;";

        var partiesSql = @"
            SELECT 
                cp.Id,
                cp.CaseId,
                cp.ClientId,
                cl.FullName AS ClientName,
                cp.PartyName,
                CASE cp.PartyRole 
                    WHEN 'Plaintiff' THEN 0 
                    WHEN 'Defendant' THEN 1 
                    WHEN 'Witness' THEN 2 
                    WHEN 'OtherLawyer' THEN 3 
                    ELSE 4 
                END AS PartyRole,
                CASE cp.PartyRole 
                    WHEN 'Plaintiff' THEN N'مدعي' 
                    WHEN 'Defendant' THEN N'مدعى عليه' 
                    WHEN 'Witness' THEN N'شاهد' 
                    WHEN 'OtherLawyer' THEN N'محامي الخصم' 
                    ELSE N'أخرى' 
                END AS PartyRoleName,
                cp.IsMainClient,
                cp.Notes
            FROM CaseParties cp
            LEFT JOIN Clients cl ON cp.ClientId = cl.Id
            WHERE cp.CaseId = @Id;";

        var assignmentsSql = @"
            SELECT 
                Id,
                CaseId,
                UserId,
                RoleInCase,
                AssignedDate,
                Notes
            FROM CaseAssignments
            WHERE CaseId = @Id
            ORDER BY AssignedDate DESC;";

        var timelineSql = @"
            SELECT 
                Id,
                CaseId,
                Title,
                Description,
                IsImportant,
                CreatedAt,
                CreatedBy
            FROM CaseTimelines
            WHERE CaseId = @Id
            ORDER BY CreatedAt DESC;";

        var caseDetails = await connection.QuerySingleOrDefaultAsync<CaseDetailsDto>(caseSql, new { Id = request.Id });
        if (caseDetails is null)
            return Result<CaseDetailsDto>.Failure(CaseErrors.NotFound(request.Id));

        var parties = await connection.QueryAsync<CasePartyDto>(partiesSql, new { Id = request.Id });
        var assignments = await connection.QueryAsync<CaseAssignmentDto>(assignmentsSql, new { Id = request.Id });
        var timelines = await connection.QueryAsync<CaseTimelineDto>(timelineSql, new { Id = request.Id });

        var fullDetails = caseDetails with
        {
            Parties = parties,
            Assignments = assignments,
            Timelines = timelines
        };

        return Result<CaseDetailsDto>.Success(fullDetails);
    }
}
