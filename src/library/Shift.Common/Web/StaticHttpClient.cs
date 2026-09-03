using System.Net.Http;
using System.Threading;

namespace Shift.Common
{
    public static class StaticHttpClient
    {
        public static readonly HttpClient Client = new HttpClient();

        public static readonly HttpClient ClientNoTimeout = new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        };
    }
}
