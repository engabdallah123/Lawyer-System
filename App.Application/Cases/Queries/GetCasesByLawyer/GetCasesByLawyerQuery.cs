using App.Application.Cases.DTOs;
using Shared.Application.Messaging;

namespace App.Application.Cases.Queries.GetCasesByLawyer;

public record GetCasesByLawyerQuery(string UserId, bool? IsClosed = false) : IQuery<IEnumerable<CaseDto>>;
