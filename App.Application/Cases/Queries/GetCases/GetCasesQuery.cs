using App.Application.Cases.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Cases.Queries.GetCases;

public record GetCasesQuery(
    string? SearchTerm = null,
    int? CaseTypeId = null,
    int? CaseStatusId = null,
    int? CourtId = null,
    bool? IsClosed = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IEnumerable<CaseDto>>;
