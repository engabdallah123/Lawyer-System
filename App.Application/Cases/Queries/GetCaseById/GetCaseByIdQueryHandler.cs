using App.Application.Cases.DTOs;
using App.Domain.Cases.Enums;
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
                (
                    ISNULL((SELECT SUM(TotalAmount) FROM FeeAgreements fa WHERE fa.CaseId = c.Id), 0) +
                    ISNULL((SELECT SUM(TotalAmount) FROM Invoices inv WHERE inv.CaseId = c.Id AND inv.FeeAgreementId IS NULL), 0)
                ) AS TotalFees,
                ISNULL((SELECT SUM(Amount) FROM Payments p WHERE p.CaseId = c.Id), 0) AS PaidFees,
                CASE 
                    WHEN (
                        ISNULL((SELECT SUM(TotalAmount) FROM FeeAgreements fa WHERE fa.CaseId = c.Id), 0) +
                        ISNULL((SELECT SUM(TotalAmount) FROM Invoices inv WHERE inv.CaseId = c.Id AND inv.FeeAgreementId IS NULL), 0)
                    ) >= ISNULL((SELECT SUM(Amount) FROM Payments p WHERE p.CaseId = c.Id), 0)
                    THEN (
                        ISNULL((SELECT SUM(TotalAmount) FROM FeeAgreements fa WHERE fa.CaseId = c.Id), 0) +
                        ISNULL((SELECT SUM(TotalAmount) FROM Invoices inv WHERE inv.CaseId = c.Id AND inv.FeeAgreementId IS NULL), 0)
                    ) - ISNULL((SELECT SUM(Amount) FROM Payments p WHERE p.CaseId = c.Id), 0)
                    ELSE 0
                END AS RemainingFees
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
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                cp.PartyName,
                cp.PartyType,
                CASE cp.PartyRole 
                    WHEN 'Plaintiff' THEN 0 
                    WHEN 'Defendant' THEN 1 
                    WHEN 'Appellant' THEN 2 
                    WHEN 'Appellee' THEN 3 
                    WHEN 'Petitioner' THEN 4 
                    WHEN 'Respondent' THEN 5 
                    WHEN 'Accused' THEN 6 
                    WHEN 'Victim' THEN 7 
                    WHEN 'Witness' THEN 8 
                    WHEN 'Expert' THEN 9 
                    WHEN 'Partner' THEN 10 
                    WHEN 'Creditor' THEN 11 
                    WHEN 'Debtor' THEN 12 
                    WHEN 'OtherLawyer' THEN 13 
                    ELSE 14 
                END AS PartyRole,
                CASE cp.PartyRole 
                    WHEN 'Plaintiff' THEN N'مدعي' 
                    WHEN 'Defendant' THEN N'مدعى عليه' 
                    WHEN 'Appellant' THEN N'مستأنف' 
                    WHEN 'Appellee' THEN N'مستأنف ضده' 
                    WHEN 'Petitioner' THEN N'طاعن' 
                    WHEN 'Respondent' THEN N'مطعون ضده' 
                    WHEN 'Accused' THEN N'متهم' 
                    WHEN 'Victim' THEN N'مجني عليه' 
                    WHEN 'Witness' THEN N'شاهد' 
                    WHEN 'Expert' THEN N'خبير' 
                    WHEN 'Partner' THEN N'خصم متدخل / شريك' 
                    WHEN 'Creditor' THEN N'دائن' 
                    WHEN 'Debtor' THEN N'مدين' 
                    WHEN 'OtherLawyer' THEN N'محامي الطرف الآخر' 
                    ELSE N'أخرى' 
                END AS PartyRoleName,
                cp.IsMainClient,
                cp.Address,
                cp.Phone,
                cp.Email,
                cp.LawyerName,
                cp.LawyerPhone,
                cp.Notes
            FROM CaseParties cp
            LEFT JOIN Clients cl ON cp.ClientId = cl.Id
            WHERE cp.CaseId = @Id;";

        var assignmentsSql = @"
            SELECT 
                ca.Id,
                ca.CaseId,
                ca.UserId,
                u.FullName,
                u.UserName,
                ca.RoleInCase,
                ca.AssignedDate,
                ca.Notes
            FROM CaseAssignments ca
            LEFT JOIN AspNetUsers u ON ca.UserId = u.Id
            WHERE ca.CaseId = @Id
            ORDER BY ca.AssignedDate DESC;";

        var timelineSql = @"
            SELECT 
                t.Id,
                t.CaseId,
                t.Title,
                CASE 
                    WHEN uAssigned.Id IS NOT NULL 
                    THEN REPLACE(REPLACE(t.Description, N'المستخدم', N'المحامي'), uAssigned.Id, ISNULL(NULLIF(uAssigned.FullName, ''), uAssigned.UserName))
                    ELSE t.Description 
                END AS Description,
                t.IsImportant,
                t.CreatedAt,
                ISNULL(NULLIF(u.FullName, ''), ISNULL(u.UserName, t.CreatedBy)) AS CreatedBy
            FROM CaseTimelines t
            LEFT JOIN AspNetUsers u ON t.CreatedBy = u.Id OR t.CreatedBy = u.UserName
            LEFT JOIN AspNetUsers uAssigned ON t.Description LIKE '%' + uAssigned.Id + '%'
            WHERE t.CaseId = @Id
            ORDER BY t.CreatedAt DESC;";

        var caseDetails = await connection.QuerySingleOrDefaultAsync<CaseDetailsDto>(caseSql, new { Id = request.Id });
        if (caseDetails is null)
            return Result<CaseDetailsDto>.Failure(CaseErrors.NotFound(request.Id));

        var parties = await connection.QueryAsync<CasePartyDto>(partiesSql, new { Id = request.Id });
        var assignments = await connection.QueryAsync<CaseAssignmentDto>(assignmentsSql, new { Id = request.Id });
        var timelines = await connection.QueryAsync<CaseTimelineDto>(timelineSql, new { Id = request.Id });

        caseDetails.Parties = parties;
        caseDetails.Assignments = assignments;
        caseDetails.Timelines = timelines;

        return Result<CaseDetailsDto>.Success(caseDetails);
    }
}
