using App.Application.Documents.DTOs;
using App.Domain.Documents.Errors;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Queries.GetDocumentById
{
    internal sealed class GetDocumentByIdQueryHandler : IQueryHandler<GetDocumentByIdQuery, DocumentDetailsDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetDocumentByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<DocumentDetailsDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var docSql = @"
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
                d.CreatedAt,
                d.CreatedBy
            FROM Documents d
            LEFT JOIN Cases c ON d.CaseId = c.Id
            LEFT JOIN Clients cl ON d.ClientId = cl.Id
            LEFT JOIN DocumentTypes dt ON d.DocumentTypeId = dt.Id
            WHERE d.Id = @Id AND d.IsDeleted = 0;";

            var versionsSql = @"
            SELECT 
                Id,
                DocumentId,
                VersionNumber,
                FilePath,
                FileName,
                ContentType,
                FileSize,
                UploadedAt,
                UploadedBy,
                Notes
            FROM DocumentVersions
            WHERE DocumentId = @Id
            ORDER BY VersionNumber DESC;";

            var document = await connection.QuerySingleOrDefaultAsync<DocumentDetailsDto>(docSql, new { Id = request.Id });
            if (document is null)
                return Result<DocumentDetailsDto>.Failure(DocumentErrors.NotFound(request.Id));

            var versions = await connection.QueryAsync<DocumentVersionDto>(versionsSql, new { Id = request.Id });

            var fullDocument = document with { Versions = versions };
            return Result<DocumentDetailsDto>.Success(fullDocument);
        }
    }
}
