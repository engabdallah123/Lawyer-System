using App.Application.Consultations.DTOs;
using App.Domain.Consultations.Errors;
using Dapper;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Consultations.Queries.GetConsultationById;

internal sealed class GetConsultationByIdQueryHandler : IQueryHandler<GetConsultationByIdQuery, ConsultationDto>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetConsultationByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<ConsultationDto>> Handle(GetConsultationByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

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
            WHERE c.Id = @Id;";

        var consultation = await connection.QuerySingleOrDefaultAsync<ConsultationDto>(sql, new { Id = request.Id });
        if (consultation is null)
            return Result<ConsultationDto>.Failure(ConsultationErrors.NotFound(request.Id));

        return Result<ConsultationDto>.Success(consultation);
    }
}
