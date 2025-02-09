using Microsoft.Extensions.DependencyInjection;

/* Unmerged change from project 'SubSonic.Configuration (net6.0)'
Before:
using System;
After:
using SubSonic;
using SubSonic.Configuration;
using SubSonic.Configuration;
using SubSonic.Configuration.Interfaces;
using System;
*/
using System;

namespace SubSonic.Configuration
{
    public interface IBuilder<TOptions>
        : IBuilder
        where TOptions : class, ISubSonicOptions
    {
        TOptions Options { get; }

        IBuilder Configure(Action<TOptions> options);
    }

    public interface IBuilder
    {
        bool Enabled { get; }

        IServiceCollection Services { get; }
        string Name { get; }
    }
}
