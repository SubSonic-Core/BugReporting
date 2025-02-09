using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace SubSonic.Configuration
{
    /// <summary>
    /// tag any provider class with this interface
    /// </summary>
    public interface IProvider
        : IDisposable
    {
        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder builder, ILogger? logger = null);
    }
}
