using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubSonic.Testing.Mocks
{
    public class MockTestRigDelegatingHandler
        : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _terminatingHandler;

        public HttpRequestHeaders? RequestHeaders { get; private set; }

        public HttpResponseHeaders? ResponseHeaders { get; private set; }

        public static MockTestRigDelegatingHandler NoContentResponse(Action<HttpRequestMessage>? theRequest = null) => new MockTestRigDelegatingHandler((request, token) =>
        {
            theRequest?.Invoke(request);

            request?.Dispose();

            return GetHttpResponse(HttpStatusCode.NoContent);
        });

        public static MockTestRigDelegatingHandler ContentResponse(HttpStatusCode statusCode, string? content, Encoding? encoding = null, string mediaType = null!) => new MockTestRigDelegatingHandler((request, token) =>
        {
            return GetHttpResponse(statusCode, content, encoding, mediaType);
        });

        public MockTestRigDelegatingHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> terminatingHandler)
        {
            _terminatingHandler = terminatingHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestHeaders = request.Headers;

            var response = await _terminatingHandler.Invoke(request, cancellationToken);

            ResponseHeaders = response.Headers;

            return response;
        }

#if NET8_0_OR_GREATER
        protected static Task<HttpResponseMessage> GetHttpResponse(HttpStatusCode statusCode, string? content = null, Encoding? encoding = null, string mediaType = null!)
#else
        protected static Task<HttpResponseMessage> GetHttpResponse(HttpStatusCode statusCode, string? content = null, Encoding? encoding = null, string? mediaType = null)
#endif
        {
            switch (statusCode)
            {
                case HttpStatusCode.NoContent:
                    return Task.Factory.StartNew(() => new HttpResponseMessage(statusCode));
                default:
                    return Task.Factory.StartNew(() =>
                    {
                        if (content != null)
                        {
                            if (encoding == null)
                            {
                                throw new ArgumentNullException(nameof(encoding));
                            }

                            if (mediaType == null)
                            {
                                throw new ArgumentNullException(nameof(mediaType));
                            }

                            HttpContent httpContent = new StringContent(content, encoding, mediaType);

                            return new HttpResponseMessage(statusCode)
                            {
                                Content = httpContent
                            };
                        }

                        return new HttpResponseMessage(statusCode);
                    });
            }
        }
    }
}
