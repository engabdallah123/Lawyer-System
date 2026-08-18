using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Database;
using Shared.Application.IService;
using Shared.Infrastructure.Database;
using Shared.Infrastructure.Services;

namespace Shared.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));

            #region Dapper
            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
            #endregion

            #region Caching
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            #endregion

            services.AddScoped<IFileService, FileService>();

            return services;
        }
    }
}
