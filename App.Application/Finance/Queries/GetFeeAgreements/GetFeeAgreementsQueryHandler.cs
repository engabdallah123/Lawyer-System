using App.Application.Finance.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Finance.Queries.GetFeeAgreements;

internal sealed class GetFeeAgreementsQueryHandler : IQueryHandler<GetFeeAgreementsQuery, IEnumerable<FeeAgreementDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetFeeAgreementsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<FeeAgreementDto>>> Handle(GetFeeAgreementsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                fa.Id,
                fa.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                fa.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                CASE fa.AgreementType 
                    WHEN 'Fixed' THEN 0 
                    WHEN 'Percentage' THEN 1 
                    WHEN 'Hourly' THEN 2 
                    ELSE 0 
                END AS AgreementType,
                CASE fa.AgreementType 
                    WHEN 'Fixed' THEN N'مبلغ ثابت' 
                    WHEN 'Percentage' THEN N'نسبة مئوية' 
                    WHEN 'Hourly' THEN N'بالساعة' 
                    ELSE N'غير محدد' 
                END AS AgreementTypeName,
                fa.TotalAmount,
                fa.PaidAmount,
                (fa.TotalAmount - fa.PaidAmount) AS RemainingAmount,
                fa.Description,
                fa.StartDate,
                fa.EndDate,
                fa.CreatedAt,
                fa.CreatedBy
            FROM FeeAgreements fa
            INNER JOIN Clients cl ON fa.ClientId = cl.Id
            LEFT JOIN Cases c ON fa.CaseId = c.Id
            WHERE (@ClientId IS NULL OR fa.ClientId = @ClientId)
                AND (@CaseId IS NULL OR fa.CaseId = @CaseId)
            ORDER BY fa.StartDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var agreements = await connection.QueryAsync<FeeAgreementDto>(
            sql,
            new
            {
                ClientId = request.ClientId,
                CaseId = request.CaseId,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<FeeAgreementDto>>.Success(agreements);
    }
}
