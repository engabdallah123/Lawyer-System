using App.Application.Lookups.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Lookups.Queries.GetCaseStatuses
{
    internal sealed class GetCaseStatusesQueryHandler : IQueryHandler<GetCaseStatusesQuery, IEnumerable<LookupDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCaseStatusesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<LookupDto>>> Handle(GetCaseStatusesQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var sql = "SELECT Id, Name, NULL AS Description, Color AS Extra FROM CaseStatuses WHERE (@OnlyActive = 0 OR IsActive = 1) ORDER BY Id;";
            var statuses = await connection.QueryAsync<LookupDto>(sql, new { OnlyActive = request.OnlyActive });
            return Result<IEnumerable<LookupDto>>.Success(statuses);
        }
    }
}
