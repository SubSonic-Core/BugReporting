using Microsoft.Extensions.DependencyInjection;
using SubSonic.Configuration;

namespace SubSonic.Notification
{
    public class NotificationBuilder
        : PackageProviderBuilder<NotificationOptions>
    {
        public override bool Enabled => Options.Enabled;

        public NotificationBuilder(IServiceCollection services, string name)
            : base(services, name) 
        { }

        public override IBuilder Configure(Action<NotificationOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            // initialize option class for pre-build operations
            configure.Invoke(Options);

            Services
                .AddOptions<NotificationOptions>(Name)
                .Configure(SetConfiguration);

            return this;
        }
    }
}
