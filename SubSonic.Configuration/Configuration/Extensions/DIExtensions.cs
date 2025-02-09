using Microsoft.Extensions.DependencyInjection;
using SubSonic.Configuration.Options;
using System;

namespace SubSonic.Configuration
{
    public static class DIExtensions
    {
        public static IServiceCollection AddEnvironment(this IServiceCollection services, Action<EnvironmentOptions> config)
        {
            services.Configure(config);

            services.AddSingleton<SubSonicEnvironment>();

            return services;
        }
    }
}
