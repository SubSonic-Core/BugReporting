using Microsoft.Extensions.DependencyInjection;
using SubSonic.Cache;
using System;

namespace SubSonic.Security
{
    public static class PackageExt
    {
        public static IServiceCollection AddSubSonicAuthentication(this IServiceCollection services, Action<SubSonicSecurityOptions> security)
        {
            var options = new SubSonicSecurityOptions();

            security.Invoke(options);

            services
                .Configure(security)
                .Configure<ExpiringCacheOptions>(nameof(Tokens.AuthenticationTokens), (cache) =>
                {
                    cache.ExpiresIn = options.TokensExpireInSeconds;
                })
                .AddSingleton<IExpiringCache<string, Tokens.AuthenticationTokens>, ExpiringCacher<string, Tokens.AuthenticationTokens>>()
                .AddTransient<Tokens.IAuthenticationTokensAccessor, CredentialManager>();

            services
                .AddAuthentication(SubSonicBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options.AddJwtBearer);

            return services;
        }
    }
}
