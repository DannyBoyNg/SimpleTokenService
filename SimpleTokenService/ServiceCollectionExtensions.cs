using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ng.Services
{
    /// <summary>
    /// Contains static methods to help with dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the simple token service.
        /// </summary>
        /// <param name="serviceCollection">DI container.</param>
        /// <returns>DI container.</returns>
        public static IServiceCollection AddSimpleTokenService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISimpleTokenService, SimpleTokenService>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the simple token service.
        /// </summary>
        /// <param name="serviceCollection">DI container.</param>
        /// <param name="options">The options.</param>
        /// <returns>DI container.</returns>
        public static IServiceCollection AddSimpleTokenService(this IServiceCollection serviceCollection, Action<SimpleTokenSettings> options)
        {
            serviceCollection.AddScoped<ISimpleTokenService, SimpleTokenService>();
            serviceCollection.Configure(options);
            return serviceCollection;
        }
    }
}
