using System.Net.Http;

namespace Shift.Common
{
    public static class StaticHttpClient
    {
        public static readonly HttpClient Client = new HttpClient();
    }
}