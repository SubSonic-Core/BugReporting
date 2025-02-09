using NSubstitute;
using SubSonic.Configuration.Http;
using SubSonic.Testing.Mocks;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubSonic.Testing.TestFixtures
{
    public abstract class HttpClientTestFixture<THttpClient>
        : OptionsTestFixture
    {
        protected MockMicrosoftLogger<THttpClient>? _logger;
        protected IHttpClientFactory? _httpClientFactory;
        protected MockTestRigDelegatingHandler? _testRigDelegatingHandler;

        protected virtual Uri BaseAddress => new Uri("https://localhost/");

        protected virtual void SetUp()
        {
            _logger = Substitute.ForPartsOf<MockMicrosoftLogger<THttpClient>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
        }

        protected virtual void TearDown()
        {
            _testRigDelegatingHandler?.Dispose();
            _testRigDelegatingHandler = null;
        }

        protected void SetupHttpResponseHandler()
        {
            _testRigDelegatingHandler = MockTestRigDelegatingHandler.NoContentResponse();
        }

        protected void SetupHttpResponseHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _testRigDelegatingHandler = new MockTestRigDelegatingHandler(handler);
        }

        protected void SetupHttpResponseHandler(HttpStatusCode statusCode, string? content = "", string mediaType = "application/json")
        {
            _testRigDelegatingHandler = MockTestRigDelegatingHandler.ContentResponse(statusCode, content, Encoding.UTF8, mediaType);
        }

        protected void SetupHttpClient(string name = HttpClientDefaults.DefaultClient)
        {
            if (_testRigDelegatingHandler == null)
            {
                SetupHttpResponseHandler();
            }

            SetupHttpClient(name, _testRigDelegatingHandler!);
        }

        protected void SetupHttpClient(string name = HttpClientDefaults.DefaultClient, params DelegatingHandler[] handlers)
        {
            if (_httpClientFactory == null)
            {
                throw new InvalidOperationException("override the SetUp and TearDown and tag them with nunit attributes.");
            }

            for (var i = 1; i < handlers.Length; i++)
            {
                handlers[i - 1].InnerHandler = handlers[i];
            }

            if (!(handlers.LastOrDefault() is MockTestRigDelegatingHandler))
            {   // the last handler in the chain is not our test rig
                _testRigDelegatingHandler ??= MockTestRigDelegatingHandler.NoContentResponse();

                SetupHttpClient(name, handlers.Append(_testRigDelegatingHandler).ToArray());

                return;
            }

            _httpClientFactory.CreateClient(Arg.Is(name))
                .Returns(new HttpClient(handlers.First())
                {
                    BaseAddress = BaseAddress
                });
        }
    }
}
