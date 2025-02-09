using Microsoft.Extensions.DependencyInjection;
using SubSonic.Configuration;

namespace SubSonic.Notification
{
    public class NotificationProviderFactory
        : IFactory<INotificationProvider, INotifier>
    {
        private bool _disposed;
        private bool _initialized;
        private IList<INotificationProvider>? _providers;

        private readonly IServiceProvider _provider;
        private readonly IDictionary<string, ProviderImplementation> _register;

        internal NotificationProviderFactory(IServiceProvider provider, IDictionary<string, ProviderImplementation> register) 
        {
            _provider = provider;
            _register = register;
        }

        public IEnumerable<INotificationProvider> Providers => _providers ?? new List<INotificationProvider>();

        public bool Enabled => (_providers?.Any() ?? false);

        public bool Initialized => _initialized;

        public INotifier Create<TCategory>() where TCategory : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach(var provider in Providers)
                {
                    provider.Dispose();
                }
            }
        }

        public void Initialize()
        {
            if (_initialized) return;

            _providers = new List<INotificationProvider>();

            foreach (var key in _register.Keys)
            {
                var service = _provider.GetRequiredService(_register[key].Type);

                if (service != null && 
                    service is INotificationProvider provider) 
                {
                    _providers.Add(provider);
                }
            }

            _initialized = true;
        }
    }
}
