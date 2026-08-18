using App.Application.Consultations.DTOs;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Consultations.Queries.GetConsultations;

internal sealed class GetConsultationsQueryHandler : IQueryHandler<GetConsultationsQuery, IEnumerable<ConsultationDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetConsultationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<ConsultationDto>>> Handle(GetConsultationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var statusName = request.Status switch
        {
            0 => "Scheduled",
            1 => "Completed",
            2 => "Cancelled",
            _ => null
        };

        var sql = @"
            SELECT 
                c.Id,
                c.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                cl.Phone AS ClientPhone,
                c.ConsultationDate,
                c.Subject,
                c.Description,
                c.Fee,
                CASE c.Status 
                    WHEN 'Scheduled' THEN 0 
                    WHEN 'Completed' THEN 1 
                    WHEN 'Cancelled' THEN 2 
                    ELSE 0 
                END AS Status,
                CASE c.Status 
                    WHEN 'Scheduled' THEN N'مجدولة' 
                    WHEN 'Completed' THEN N'مكتملة' 
                    WHEN 'Cancelled' THEN N'ملغاة' 
                    ELSE N'غير محدد' 
                END AS StatusName,
                c.Notes,
                c.CreatedAt,
                c.CreatedBy
            FROM Consultations c
            INNER JOIN Clients cl ON c.ClientId = cl.Id
            WHERE (@ClientId IS NULL OR c.ClientId = @ClientId)
                AND (@Status IS NULL OR c.Status = @StatusName)
                AND (@FromDate IS NULL OR c.ConsultationDate >= @FromDate)
                AND (@ToDate IS NULL OR c.ConsultationDate <= @ToDate)
            ORDER BY c.ConsultationDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var consultations = await connection.QueryAsync<ConsultationDto>(
            sql,
            new
            {
                ClientId = request.ClientId,
                Status = request.Status,
                StatusName = statusName,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Offset = offset,
                PageSize = request.PageSize
            });

        return Result<IEnumerable<ConsultationDto>>.Success(consultations);
    }
}
