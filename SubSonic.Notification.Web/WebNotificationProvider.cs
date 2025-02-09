using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Azure.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SubSonic.Configuration;

namespace SubSonic.Notification
{
    public class WebNotificationProvider
        : ProviderWithOptions<WebNotificationOptions>
        , INotificationProvider
    {
        bool _disposed = false;

        public WebNotificationProvider(WebNotificationOptions options) 
            : base(options)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSignalR(Options.SetActionOptions<HubOptions>())
                .If(Options.UseStackExchangeRedis, builder =>
                {
                    builder.AddStackExchangeRedis(Options.SetActionOptions<RedisOptions>());
                })
                .If(Options.UseAzureServices, builder =>
                {
                    builder.AddAzureSignalR(Options.SetActionOptions<ServiceOptions>());
                });

            services.AddSingleton<INotificationChannel, WebNotificationChannel>();
        }

        public override void Configure(IApplicationBuilder builder, ILogger? logger = null)
        {
            if (builder is IEndpointRouteBuilder endpoints)
            {   // if builder is WebApplication it supports IEndpointRouteBuilder
                endpoints.MapHub<NotificationHub>($"/hub/{Options.Endpoint}", Options.SetActionOptions<HttpConnectionDispatcherOptions>());
            }
            else
            {   // if builder is ApplicationBuilder it does not support IEndpointRouteBuilder
                if (!builder.IsMiddlewareAdded("EndpointRoutingMiddleware"))
                {
                    builder.UseRouting();
                }

                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<NotificationHub>($"/hub/{Options.Endpoint}", Options.SetActionOptions<HttpConnectionDispatcherOptions>());
                });
            }

            logger?.LogDebug("NotificationHub mapped to {Endpoint}", $"/hub/{Options.Endpoint}");
        }           

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
