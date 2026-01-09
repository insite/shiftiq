using System.Net;

namespace Shift.Common
{
    public static class HttpWebResponseExtensions
    {
        public static HttpWebResponse GetResponseNoException(this HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                HttpWebResponse response = we.Response as HttpWebResponse;
                if (response == null)
                    throw;
                return response;
            }
        }
    }
}