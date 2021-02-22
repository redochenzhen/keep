using Keep.Common.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Keep.Common.Tests
{
    public class TimeoutHandlerTests
    {
        [Fact]
        public async void TimeoutTests()
        {
            var http = new HttpClient(TimeoutHandler.Create());
            var request = new HttpRequestMessage(HttpMethod.Get, "http://baidu.com");
            request.SetTimeout(TimeSpan.FromMilliseconds(1));
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await http.SendAsync(request);
            });
        }

        [Fact]
        public async void LocalTimeoutTests()
        {
            var http = new HttpClient(TimeoutHandler.Create());
            http.Timeout = TimeSpan.FromMilliseconds(1);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://baidu.com");
            request.SetTimeout(TimeSpan.FromMilliseconds(100));
            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await http.SendAsync(request);
            });
        }

        [Fact]
        public async void CanceledByUserTests()
        {
            var http = new HttpClient(TimeoutHandler.Create());
            http.Timeout = TimeSpan.FromMilliseconds(100);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://baidu.com");
            var cts = new CancellationTokenSource(1);
            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await http.SendAsync(request, cts.Token);
            });
        }
    }
}
