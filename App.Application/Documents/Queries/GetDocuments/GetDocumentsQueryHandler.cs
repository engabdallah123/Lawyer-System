using App.Application.Documents.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Documents.Queries.GetDocuments;

internal sealed class GetDocumentsQueryHandler : IQueryHandler<GetDocumentsQuery, IEnumerable<DocumentDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetDocumentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IEnumerable<DocumentDto>>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                d.Id,
                d.CaseId,
                c.InternalNumber AS CaseInternalNumber,
                c.Title AS CaseTitle,
                d.ClientId,
                ISNULL(cl.FullName, cl.CompanyName) AS ClientName,
                d.DocumentTypeId,
                dt.Name AS DocumentTypeName,
                d.Name,
                d.Description,
                d.CurrentVersionId,
                ISNULL(dv.VersionNumber, 1) AS CurrentVersionNumber,
                dv.FilePath AS CurrentFilePath,
                dv.FileName AS CurrentFileName,
                dv.FileSize AS CurrentFileSize,
                d.CreatedAt,
                d.CreatedBy
            FROM Documents d
            LEFT JOIN Cases c ON d.CaseId = c.Id
            LEFT JOIN Clients cl ON d.ClientId = cl.Id
            LEFT JOIN DocumentTypes dt ON d.DocumentTypeId = dt.Id
            LEFT JOIN DocumentVersions dv ON d.CurrentVersionId = dv.Id
            WHERE d.IsDeleted = 0
              AND (@CaseId IS NULL OR d.CaseId = @CaseId)
              AND (@ClientId IS NULL OR d.ClientId = @ClientId)
              AND (@DocumentTypeId IS NULL OR d.DocumentTypeId = @DocumentTypeId)
              AND (@SearchTerm IS NULL OR d.Name LIKE '%' + @SearchTerm + '%' OR d.Description LIKE '%' + @SearchTerm + '%')
            ORDER BY d.CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (request.Page - 1) * request.PageSize;

        var documents = await connection.QueryAsync<DocumentDto>(sql, new
        {
            CaseId = request.CaseId,
            ClientId = request.ClientId,
            DocumentTypeId = request.DocumentTypeId,
            SearchTerm = request.SearchTerm,
            Offset = offset,
            PageSize = request.PageSize
        });

        return Result<IEnumerable<DocumentDto>>.Success(documents);
    }
}
