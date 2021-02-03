using System;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Keep.Common.Http
{
    public static class HttpRequestMessageExts
    {
        private const string TIMEOUT_KEY = "X-RequestTimeout";

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan timeout, bool force = false)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (force || !request.Properties.ContainsKey(TIMEOUT_KEY))
            {
                request.Properties[TIMEOUT_KEY] = timeout;
            }
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!request.Properties.TryGetValue(TIMEOUT_KEY, out var value)) return null;
            if (!(value is TimeSpan timeout)) return null;
            return timeout;
        }
    }
}
