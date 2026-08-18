using App.Application.Lookups.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Lookups.Queries.GetCaseTypes
{
    public record GetCaseTypesQuery(bool OnlyActive = true) : IQuery<IEnumerable<LookupDto>>;
}
