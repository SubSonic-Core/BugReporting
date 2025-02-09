using Microsoft.AspNetCore.SignalR;
using StackExchangeRedis = Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using AzureSignalR = Microsoft.Azure.SignalR;
using SubSonic.Configuration;

namespace SubSonic.Notification
{
    public static class PackageExt
    {
        public static IProviderBuilder AddWebNotification(this IProviderBuilder builder) =>
            builder.AddWebNotification("notification", null!);

        public static IProviderBuilder AddWebNotification(this IProviderBuilder builder, Action<WebNotificationOptions> options) =>
            builder.AddWebNotification("notification", options);

        public static IProviderBuilder AddWebNotification(this IProviderBuilder builder, string endpoint, Action<WebNotificationOptions> options)
        {
            var options_ = new WebNotificationOptions(endpoint);

            options?.Invoke(options_);

            builder.AddProvider<WebNotificationProvider>(provider =>
            {
                return new WebNotificationProvider(options_);
            });

            return builder; 
        }

        public static WebNotificationOptions AddHubOptions(this WebNotificationOptions src, Action<HubOptions> options)
        {
            src.SetOptions(options);

            return src;
        }

        public static WebNotificationOptions AddStackExchangeRedis(this WebNotificationOptions src, Action<StackExchangeRedis.RedisOptions> options)
        {
            if (src.UseAzureServices)
            {
                throw new InvalidOperationException("Azure Services is already configured.");
            }

            src.SetOptions(options);

            return src;
        }

        public static WebNotificationOptions AddAzureServices(this WebNotificationOptions src, Action<AzureSignalR.ServiceOptions> options)
        {
            if (src.UseStackExchangeRedis)
            {
                throw new InvalidOperationException("Stack Exchange Redis is already configured.");
            }

            src.SetOptions(options);

            return src;
        }
    }
}
