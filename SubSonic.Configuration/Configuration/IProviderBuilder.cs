using System;

/* Unmerged change from project 'SubSonic.Configuration (net6.0)'
Before:
using System.Collections.Generic;
After:
using System.Collections.Generic;
using SubSonic;
using SubSonic.Configuration;
using SubSonic.Configuration;
using SubSonic.Configuration.Interfaces;
*/
using System.Collections.Generic;

namespace SubSonic.Configuration
{
    public interface IProviderBuilder<TOptions>
        : IBuilder<TOptions>
        , IProviderBuilder
        where TOptions : class, ISubSonicOptions, new()
    { }

    public interface IProviderBuilder
        : IBuilder
    {
        IDictionary<string, ProviderImplementation> Providers { get; }

        IBuilder AddProvider<TProvider>() where TProvider : class, IProvider;

        IBuilder AddProvider<TProvider>(Func<IServiceProvider, object> provider) where TProvider : class, IProvider;
    }
}
