using App.Application.Lookups.DTOs;
using Shared.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Lookups.Queries.GetCaseStatuses
{
    public record GetCaseStatusesQuery(bool OnlyActive = true) : IQuery<IEnumerable<LookupDto>>;
}
