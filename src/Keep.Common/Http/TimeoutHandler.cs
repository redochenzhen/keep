using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Common.Http
{
    public class TimeoutHandler : DelegatingHandler
    {
        public const string TIMEOUT_KEY = "X-RequestTimeout";

        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(100);

        public static TimeoutHandler Create(HttpMessageHandler innerHandler = default)
        {
            return new TimeoutHandler
            {
                InnerHandler = innerHandler ?? new HttpClientHandler()
            };
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage httpRequestMessage,
            CancellationToken cancellationToken)
        {
            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException(nameof(httpRequestMessage));
            }
            using (var cts = GetCancellationTokenSource(httpRequestMessage, cancellationToken))
            {
                try
                {
                    return await base.SendAsync(httpRequestMessage, cts?.Token ?? cancellationToken);
                }
                catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException();
                }
            }
        }

        private CancellationTokenSource GetCancellationTokenSource(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var timeout = request.GetTimeout() ?? DefaultTimeout;
            if (timeout == default) timeout = DefaultTimeout;
            else if (timeout == Timeout.InfiniteTimeSpan) return null;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);
            return cts;
        }
    }
}
