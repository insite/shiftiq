using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace InSite.Web
{
    public class ServerErrorActionResult : IHttpActionResult
    {
        private string _error;
        private HttpRequestMessage _request;
        private HttpStatusCode _status = HttpStatusCode.InternalServerError;

        public ServerErrorActionResult(string error, HttpRequestMessage request, HttpStatusCode status)
        {
            _error = error;
            _request = request;
            _status = status;
        }

        public ServerErrorActionResult(string error, HttpAuthenticationContext context, HttpStatusCode status)
            : this(error, context.Request, status)
        {
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = _status,
                Content = JsonContent.Create(new { Error = _error }),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

        public static HttpResponseMessage CreateResponse(string error, HttpStatusCode status)
        {
            return new HttpResponseMessage(status)
            {
                ReasonPhrase = status.ToString(),
                Content = JsonContent.Create(new { Error = error }),
            };
        }
    }
}