using Microsoft.Extensions.Options;
using SubSonic.Testing.Mocks;
using NSubstitute;
using System;
using SubSonic.Configuration;

namespace SubSonic.Testing.TestFixtures
{
    public abstract class OptionsTestFixture
        : SubSonicOptions
    {
        public IOptions<TOptions> GetOptionsFor<TOptions>()
            where TOptions : class, new() => new MockOptions<TOptions>(GetOptions<TOptions>());

        public IOptions<TOptions> GetOptionsFor<TOptions>(string name)
            where TOptions : class, new() => new MockOptions<TOptions>(GetOptions<TOptions>(name));
    }
}
