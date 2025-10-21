using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Shift.Common;

namespace InSite.Api
{
    public static class HttpHelper
    {
        public static string ReadRequestBody(HttpRequestMessage request)
        {
            using (var stream = request.Content.ReadAsStreamAsync().Result)

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static HttpResponseMessage CreateResponse(HttpRequestMessage request, HttpStatusCode code, string content)
        {
            var response = request.CreateResponse(code);

            response.Content = new StringContent(content, Encoding.UTF8, "text/plain");

            return response;
        }

        public static HttpResponseMessage Ok(HttpRequestMessage request)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            return response;
        }

        public static HttpResponseMessage NotFound(HttpRequestMessage request, Exception ex)
        {
            var problem = ProblemFactory.InternalServerError(ex.GetFormattedMessages());

            var json = problem.Serialize();

            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/problem+json")
            };

            return response;
        }

        public static HttpResponseMessage ServerError(HttpRequestMessage request, Exception ex)
        {
            var problem = ProblemFactory.InternalServerError(ex.GetFormattedMessages());

            var json = problem.Serialize();

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/problem+json")
            };

            return response;
        }
    }
}