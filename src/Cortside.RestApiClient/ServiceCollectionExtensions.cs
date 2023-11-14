using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.RestApiClient {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddRestApiClient<TInterface, TImplementation, TConfiguration>(this IServiceCollection services, IConfiguration configuration, string key) where TImplementation : class, TInterface where TInterface : class where TConfiguration : class, IConfiguration {
            var config = configuration.GetSection(key).Get<TConfiguration>();
            services.AddSingleton(config);
            services.AddSingleton<TInterface, TImplementation>();

            return services;
        }

        public static IServiceCollection AddRestApiClient<TInterface, TImplementation, TConfiguration>(this IServiceCollection services, TConfiguration configuration) where TImplementation : class, TInterface where TInterface : class where TConfiguration : class {
            services.AddSingleton<TConfiguration>(configuration);
            services.AddSingleton<TInterface, TImplementation>();

            return services;
        }
    }
}
