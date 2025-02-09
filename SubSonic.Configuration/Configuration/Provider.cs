using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SubSonic.Configuration
{
    public abstract class Provider
        : IProvider
    {
        public abstract void ConfigureServices(IServiceCollection services);

        public abstract void Configure(IApplicationBuilder builder, ILogger? logger = null);

        public abstract void Dispose();
    }
}
