using System;

namespace SubSonic.Configuration
{
    public class ProviderImplementation
    {
        public static ProviderImplementation Create<TProvider>() 
            where TProvider: class, IProvider
        {
            return new ProviderImplementation(typeof(TProvider), null);
        }

        public static ProviderImplementation Create<TProvider>(Func<IServiceProvider, object> provider)
             where TProvider : class, IProvider
        {
            return new ProviderImplementation(typeof(TProvider), provider);
        }

        protected internal ProviderImplementation(Type type, Func<IServiceProvider, object>? provider) {
            Type = type;
            Provider = provider;
        }

        public Type Type { get; }

        public Func<IServiceProvider, object>? Provider { get; }
    }
}
