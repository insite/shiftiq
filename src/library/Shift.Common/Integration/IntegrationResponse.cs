using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Shift.Common
{
    public class IntegrationResponse
    {
        public WebHeaderCollection Headers { get; private set; }
        public string ContentType { get; private set; }
        public string Content { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public IntegrationResponse(HttpWebResponse response, string content)
        {
            Headers = response.Headers;
            ContentType = response.ContentType;
            Content = content;
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
        }

        public IntegrationResponse(HttpResponseMessage response, string content)
        {
            Headers = ConvertToWebHeaderCollection(response.Headers);
            if (response.Headers.TryGetValues("Content-Type", out var contentTypeValues))
            {
                string contentType = string.Join(",", contentTypeValues);
                ContentType = contentType;
            }
            Content = content;
            StatusCode = response.StatusCode;
            StatusDescription = response.ReasonPhrase;
        }

        public static WebHeaderCollection ConvertToWebHeaderCollection(HttpResponseHeaders responseHeaders)
        {
            WebHeaderCollection webHeaderCollection = new WebHeaderCollection();

            foreach (var header in responseHeaders)
            {
                string headerName = header.Key;
                string headerValue = string.Join(",", header.Value);
                webHeaderCollection[headerName] = headerValue;
            }

            return webHeaderCollection;
        }
    }
}
