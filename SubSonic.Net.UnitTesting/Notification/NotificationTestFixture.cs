using FluentAssertions;
using Host;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubSonic.Notification;
using SubSonic.Security.Tokens;
using SubSonic.Testing.TestFixtures;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubSonic.Net.UnitTesting.Notification
{
    using HubMethods = NotificationHub.Methods;

    public class NotificationTestFixture
        : WebApiTestFixture<Program>
    {
        private CancellationTokenSource _delayForMessageSentTokenSource;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _delayForMessageSentTokenSource = new CancellationTokenSource();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            _delayForMessageSentTokenSource?.Dispose();
        }

        protected override LogLevel DefaultLogLevel => LogLevel.Debug;

        private void DelayAsync(TimeSpan delay)
        {
            try
            {
                Task.Delay(delay, _delayForMessageSentTokenSource.Token).GetAwaiter().GetResult();
            }
            catch { }
        }

        private HubConnection CreateConnectionUsingLongPolling(WebNotificationOptions notificationHub, Func<Task<string?>>? accessTokenFactory = null) =>
            new HubConnectionBuilder()
                .WithUrl($"http://localhost/hub/{notificationHub.Endpoint}", options =>
                {
                    options.Transports = HttpTransportType.LongPolling;
                    options.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                    options.UseDefaultCredentials = true;

                    if(accessTokenFactory != null)
                    {
                        options.AccessTokenProvider = accessTokenFactory;
                    }
                })
                .Build();

        [Test]
        public async Task CanConnectClientHubToServerUsingLongPolling()
        {
            ConfigureServicesForTest((ctx, services) =>
            {
                var logLevel = Configuration.GetValue<LogLevel>("Logging:LogLevel:Default");

                services.AddNotification(builder =>
                {
                    builder.Configure(options =>
                    {
                        options.Enabled = true;
                    });

                    builder.AddWebNotification(web =>
                    {
                        web.AddHubOptions(hub =>
                        {
                        });
                    });
                });
            });

            Configure(app =>
            {
                app.UseNotificationServices();
            });

            INotifier notifier = null!;
            IAuthenticationTokensAccessor accessor = null!;

            FluentActions.Invoking(() =>
            {
                notifier = Services.GetRequiredService<INotifier>();
                accessor = Services.GetRequiredService<IAuthenticationTokensAccessor>();
            }).Should().NotThrow();

            var conn = CreateConnectionUsingLongPolling(
                Services.GetRequiredService<IOptionsMonitor<WebNotificationOptions>>().Get(Options.DefaultName),
                async () => (await accessor.GetAuthenticationTokensAsync("bob")).EncodedUserJwt);

            string expected = "this is a test";
            string recieved = "";

            conn.On<string>(HubMethods.RecieveSystemMessage, (msg) =>
            {
                Console.WriteLine($"System: {msg}");
            });

            conn.On<string>(HubMethods.RecieveMessage, (msg) =>
            {
                recieved = msg;

                _delayForMessageSentTokenSource.Cancel();
            });

            await conn.StartAsync();

            conn.ConnectionId.Should().NotBeNull();
            conn.State.Should().Be(HubConnectionState.Connected);

            await notifier.BroadCastAsync(expected);

            DelayAsync(TimeSpan.FromSeconds(60));

            recieved.Should().Be(expected);

            await conn.StopAsync();
        }

        
    }
} 
