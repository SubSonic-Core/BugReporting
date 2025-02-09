using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace SubSonic.Testing
{
    internal class WebApiTestFactory<TEntryPoint>
        : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private readonly IConfiguration _configuration;
        private readonly Action<WebHostBuilderContext, IServiceCollection> _configureServices;
        private readonly Action<IApplicationBuilder, ILogger>? _configure;
        private readonly Action<IServiceProvider>? _builtServiceProvider;

#nullable enable
        public WebApiTestFactory(IConfiguration configuration, 
            Action<WebHostBuilderContext, IServiceCollection> configureServices,
            Action<IApplicationBuilder, ILogger>? configure = null,
            Action<IServiceProvider>? builtServiceProvider = null)
        {
            _configuration = configuration;
            _configureServices = configureServices;
            _configure = configure;
            _builtServiceProvider = builtServiceProvider;
        }
#nullable disable

        protected override IHost CreateHost(IHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Settings.AspNetCoreEnvironment);

            var host = base.CreateHost(builder);
            
            _builtServiceProvider?.Invoke(host.Services);

            return host;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseConfiguration(_configuration);

            builder.ConfigureServices(_configureServices);

            builder.Configure((builder) =>
            {
                var logger = builder.ApplicationServices.GetRequiredService<ILogger<WebApplicationFactory<TEntryPoint>>>();

                _configure.Invoke(builder, logger);
            });
        }
    }
}
