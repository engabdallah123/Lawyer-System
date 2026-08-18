using App.Application.Cases.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Cases.Queries.GetCaseById;

public record GetCaseByIdQuery(Guid Id) : IQuery<CaseDetailsDto>;
