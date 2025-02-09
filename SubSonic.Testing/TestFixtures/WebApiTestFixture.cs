using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SubSonic.Configuration;
using SubSonic.Security;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SubSonic.Testing.TestFixtures
{
    public abstract class WebApiTestFixture<TEntryPoint>
        where TEntryPoint : class
    {
        private WebApiTestFactory<TEntryPoint>? _factory;
        private HttpClient? _httpClient;
        private Action<WebHostBuilderContext, IServiceCollection>? _configureServicesForTest = null;
        private Action<IApplicationBuilder>? _configure = null;

        private IConfigurationRoot? _configuration;

        public virtual void SetUp()
        {
            var builder = new ConfigurationBuilder();

            BuildServerConfiguration(builder);

            _configuration = builder.Build();

            _factory = new WebApiTestFactory<TEntryPoint>(_configuration, ConfigureServices, Configure);
        }

        public virtual void TearDown()
        {
            _configureServicesForTest = null;

            _httpClient?.Dispose();
            _httpClient = null;

            _factory?.Dispose();
            _factory = null;
        }

        protected IConfiguration Configuration => _configuration ?? throw new ArgumentNullException(nameof(Configuration));

        protected virtual string ApplicationName => "TestWebApi";
        protected virtual string Environment => SubSonicEnvironment.DevUnstable;
        protected virtual LogLevel DefaultLogLevel => LogLevel.Error;

        protected IServiceProvider Services => _factory?.Services ?? throw new InvalidOperationException();

        protected TestServer Server => _factory?.Server ?? throw new InvalidOperationException();

        protected virtual void BuildServerConfiguration(ConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection(new KeyValuePair<string, string?>[] {
                new ($"Settings:{nameof(ApplicationName)}", ApplicationName),
                new ($"Settings:{nameof(Environment)}", Environment),
                new ($"Logging:LogLevel:Default", Enum.GetName(DefaultLogLevel)),
                new ($"Logging:LogLevel:Microsoft.AspNetCore.Authentication", Enum.GetName(LogLevel.Information)),
                new ($"Logging:LogLevel:Microsoft.AspNetCore.Mvc.Testing", Enum.GetName(LogLevel.Debug)),
                new ($"Security:SymmetricSecurityKey", Guid.NewGuid().ToString().ToLower()),
                new ($"Security:TokensExpireInSeconds", "600")
            });
        }

        private void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services)
        {
            Console.WriteLine($"{nameof(Environment)}: {Environment}");

            var section = ctx.Configuration.GetRequiredSection("Settings");

            services.AddEnvironment(opt =>
            {
                opt.Environment = section.GetRequiredValue<string>(nameof(Environment));
            });

            services.AddSubSonicAuthentication(security =>
            {
                var section = Configuration.GetSection("Security");

                security.SymmetricSecurityKey = Encoding.UTF8.GetBytes(section.GetRequiredValue<string>("SymmetricSecurityKey"));
                security.TokensExpireInSeconds = TimeSpan.FromSeconds(section.GetValue("TokensExpireInSeconds", 3600));
                security.RequireHttpsMetadata = false;
                security.Issuer = "localhost";
                security.Audience = "localhost";
            });

            services.AddAuthorization();

            services
                .AddHttpContextAccessor();

            services
                .AddMvc()
                .AddApplicationPart(typeof(TEntryPoint).Assembly)
                .AddControllersAsServices();

            _configureServicesForTest?.Invoke(ctx, services);
        }

        private void Configure(IApplicationBuilder builder, ILogger logger)
        {
            builder
                .UseHttpsRedirection();

            logger.LogDebug("calling UseRouting");

            builder.UseRouting(); // this is the first call on this extension

            logger.LogDebug("Endpoint Routing Middleware Added {Value}", builder.IsMiddlewareAdded("EndpointRoutingMiddleware"));

            foreach(var property in builder.Properties)
            {
                logger.LogDebug("Builder Property: {Property}", property);
            }

            if (builder.Properties.TryGetValue("__MiddlewareDescriptions", out var value) &&
                value is IList<string> descriptions)
            {
                foreach(var description in descriptions)
                {
                    logger.LogDebug(description);
                }
            }

            builder.UseAuthentication();
            builder.UseAuthorization();

            builder.UseEndpoints(erb =>
            {
                erb.MapControllers();
            });

            _configure?.Invoke(builder);
        }

        protected void ConfigureServicesForTest(Action<WebHostBuilderContext, IServiceCollection> configureServicesForTest)
        {
            _configureServicesForTest = configureServicesForTest;
        }

        protected void Configure(Action<IApplicationBuilder> configure)
        {
            _configure = configure;
        }

        protected HttpClient GetHttpClient(params DelegatingHandler[] handlers)
        {
            return _httpClient ??= _factory?.CreateDefaultClient(handlers) ?? throw new InvalidOperationException();
        }

        protected HttpClient GetHttpClient(Uri baseAddress, params DelegatingHandler[] handlers)
        {
            return _httpClient ??= _factory?.CreateDefaultClient(baseAddress, handlers) ?? throw new InvalidOperationException();
        }
    }
}
