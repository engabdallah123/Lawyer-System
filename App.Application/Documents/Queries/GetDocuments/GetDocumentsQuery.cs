using App.Application.Documents.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Documents.Queries.GetDocuments;

public record GetDocumentsQuery(
    Guid? CaseId = null,
    Guid? ClientId = null,
    int? DocumentTypeId = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 50) : IQuery<IEnumerable<DocumentDto>>;
