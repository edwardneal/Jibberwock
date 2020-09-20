using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.DependencyInjection
{
    public static class PersistenceServiceCollectionExtensions
    {
        /// <summary>
        /// Enables the Jibberwock persistence layer to be accessed using dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The input parameter, with all persistence layers added.</returns>
        public static IServiceCollection AddJibberwockPersistence(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddScoped<DataSources.SqlServerDataSource>()
                ;
        }
    }
}
