using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using SubSonic.Testing.Mocks;
using System;
using System.Net.Http;

namespace SubSonic.Testing
{
    public static partial class Extensions
    {
        public static IServiceCollection AddTestRigHandler(this IServiceCollection services, string clientName, Action<HttpRequestMessage> request)
        {
            services.ConfigureAll<HttpClientFactoryOptions>(options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                {
                    builder.PrimaryHandler = MockTestRigDelegatingHandler.NoContentResponse(request);
                });
            });

            return services;
        }
    }
}
