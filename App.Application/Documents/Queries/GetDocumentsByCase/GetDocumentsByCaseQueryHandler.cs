using App.Application.Documents.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Queries.GetDocumentsByCase
{
    internal sealed class GetDocumentsByCaseQueryHandler : IQueryHandler<GetDocumentsByCaseQuery, IEnumerable<DocumentDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetDocumentsByCaseQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<DocumentDto>>> Handle(GetDocumentsByCaseQuery request, CancellationToken cancellationToken)
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
            WHERE d.CaseId = @CaseId AND d.IsDeleted = 0
            ORDER BY d.CreatedAt DESC;";

            var documents = await connection.QueryAsync<DocumentDto>(sql, new { CaseId = request.CaseId });
            return Result<IEnumerable<DocumentDto>>.Success(documents);
        }
    }
}
