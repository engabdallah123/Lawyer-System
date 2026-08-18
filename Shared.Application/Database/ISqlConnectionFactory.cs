using System.Data;

namespace Shared.Application.Database
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
