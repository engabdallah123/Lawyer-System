using App.Application.Lookups.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Lookups.Queries.GetCaseTypes
{
    internal sealed class GetCaseTypesQueryHandler : IQueryHandler<GetCaseTypesQuery, IEnumerable<LookupDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCaseTypesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<LookupDto>>> Handle(GetCaseTypesQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var sql = "SELECT Id, Name, Description, NULL AS Extra FROM CaseTypes WHERE (@OnlyActive = 0 OR IsActive = 1) ORDER BY Name;";
            var types = await connection.QueryAsync<LookupDto>(sql, new { OnlyActive = request.OnlyActive });
            return Result<IEnumerable<LookupDto>>.Success(types);
        }
    }
}
