using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Cryptography
{
    public static class CryptographyServiceCollectionExtensions
    {
        /// <summary>
        /// Enables the Jibberwock cryptographic libraries to be accessed using dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The input parameter, with all cryptographic libraries added.</returns>
        public static IServiceCollection AddJibberwockCryptography(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddScoped<IHashCalculator, HashCalculator>();
        }
    }
}
