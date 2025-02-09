using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubSonic.Configuration;

namespace SubSonic.Notification
{
    public static class PackageExt
    {
        public static IServiceCollection AddNotification(this IServiceCollection services, Action<IProviderBuilder<NotificationOptions>> action) => AddNotification(services, Options.DefaultName, action);

        public static IServiceCollection AddNotification(this IServiceCollection services, string name, Action<IProviderBuilder<NotificationOptions>> action)
        {
            name = name ?? Options.DefaultName;

            var builder = new NotificationBuilder(services, name);

            action?.Invoke(builder);

            builder.ConfigureServices();

            services.AddSingleton(provider => new NotificationProviderFactory(provider, builder.Providers));
            services.AddTransient<INotifier>(provider =>
            {
                var options = provider.GetRequiredService<IOptionsMonitor<NotificationOptions>>().Get(builder.Name);

                return new Notifier(provider.GetServices<INotificationChannel>(), options);
            });

            builder.RegisterProvidersWithServiceCollection();

            return services;
        }

        public static IApplicationBuilder UseNotificationServices(this IApplicationBuilder app)
        {
            var factory = app.ApplicationServices.GetRequiredService<NotificationProviderFactory>();
            var logger = app.ApplicationServices.GetService<ILogger<NotificationProviderFactory>>();

            factory.Initialize();

            foreach (var provider in factory.Providers)
            {
                provider.Configure(app, logger);
            }

            return app;
        }
    }
}
