using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroSoftOptions = Microsoft.Extensions.Options.Options;

namespace SubSonic.Configuration
{
    public abstract class PackageProviderBuilder<TOptions>
        : PackageProviderBuilder
        , IProviderBuilder<TOptions>
        where TOptions : class, ISubSonicOptions, new()
    {
        protected readonly TOptions _options;

        public PackageProviderBuilder(IServiceCollection services, string name)
            : base(services, name) { 
            _options = new TOptions();
        }

        public TOptions Options => _options;

        protected override void ConfigureServices(KeyValuePair<string, ProviderImplementation> registration)
        {
            if (!registration.Value.Type.IsAbstract)
            {
                Type 
                    providerType = registration.Value.Type,
                    providerBaseType = registration.Value.Type.BaseType!;

                if (providerBaseType?.IsGenericType ?? false)
                {   // provider base contains options arguments
                    if (providerType.GetConstructor(providerBaseType.GenericTypeArguments) == null)
                    {
                        throw new MissingMethodException($"{providerType.Name} missing constructor that matches base class generic argument types");
                    }
                    else if (Activator.CreateInstance(providerType, Options.GetOptions(providerBaseType.GenericTypeArguments)) is IProvider provider)
                    {
                        provider.ConfigureServices(Services);
                    }
                }
                else
                {
                    base.ConfigureServices(registration);
                }
            }
        }

        public abstract IBuilder Configure(Action<TOptions> options);

        public virtual void SetConfiguration(TOptions options)
        {
            foreach (var property in typeof(TOptions).GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(options, property.GetValue(_options));
                }
            }

            if (options is SubSonicOptions dst && _options is SubSonicOptions src)
            {
                dst.SetExtendedOptions(src);
            }
        }
    }

    public abstract class PackageProviderBuilder
        : PackageBuilder
        , IProviderBuilder
    {
        private readonly IDictionary<string, ProviderImplementation> _registeredProviders;
        private readonly IServiceCollection _services;

        public PackageProviderBuilder(IServiceCollection services, string name)
            : base(name)
        {
            _registeredProviders = new Dictionary<string, ProviderImplementation>();
            _services = services;
        }

        public IDictionary<string, ProviderImplementation> Providers => _registeredProviders;

        public override IServiceCollection Services => _services;

        private static string GetName<TProvider>() => typeof(TProvider).Name;

        protected virtual void ConfigureServices(KeyValuePair<string, ProviderImplementation> registration)
        {
            if (!registration.Value.Type.IsAbstract)
            {
                if (registration.Value.Type.IsAssignableTo(typeof(Provider)) &&
                    Activator.CreateInstance(registration.Value.Type) is IProvider provider)
                {
                    provider.ConfigureServices(Services);
                }
            }
        }

        public void ConfigureServices()
        {
            foreach(var provider in _registeredProviders)
            {
                ConfigureServices(provider);
            }
        }

        public IBuilder AddProvider<TProvider>()
            where TProvider : class, IProvider
        {
            _registeredProviders.TryAdd(GetName<TProvider>(), ProviderImplementation.Create<TProvider>());

            return this;
        }

        public IBuilder AddProvider<TProvider>(Func<IServiceProvider, object> provider)
            where TProvider : class, IProvider
        {
            _registeredProviders.TryAdd(GetName<TProvider>(), ProviderImplementation.Create<TProvider>(provider));

            return this;
        }

        public IBuilder RegisterProvidersWithServiceCollection()
        {
            if (Enabled)
            {
                foreach(var impl in _registeredProviders.Select(register => register.Value))
                {
                    if (impl.Provider != null)
                    {
                        Services.AddSingleton(impl.Type, impl.Provider);
                    }
                    else
                    {
                        Services.AddSingleton(impl.Type);
                    }
                }
            }

            return this;
        }
    }
}
