using App.Application.Lookups.DTOs;
using Shared.Application.Database;
using Shared.Application.Messaging;
using Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Lookups.Queries.GetCourts
{
    internal sealed class GetCourtsQueryHandler : IQueryHandler<GetCourtsQuery, IEnumerable<LookupDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCourtsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IEnumerable<LookupDto>>> Handle(GetCourtsQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var sql = "SELECT Id, Name, City AS Description, NULL AS Extra FROM Courts WHERE (@OnlyActive = 0 OR IsActive = 1) ORDER BY Name;";
            var courts = await connection.QueryAsync<LookupDto>(sql, new { OnlyActive = request.OnlyActive });
            return Result<IEnumerable<LookupDto>>.Success(courts);
        }
    }
}
