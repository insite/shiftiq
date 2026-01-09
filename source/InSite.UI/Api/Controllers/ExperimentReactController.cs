using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;

namespace InSite.Api.Controllers
{
    [DisplayName("Experiment")]
    [RoutePrefix("client")]
    [AllowAnonymous]
    public class ReactController : ApiBaseController
    {
        [HttpGet, HttpOptions]
        public HttpResponseMessage GetResource()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/react/index.html");
            var file = new FileInfo(filePath);

            if (!file.Exists)
                return JsonError(new { Message = "The ~/react/index.html is not found. Run 'npm run build' for the Shift.UI project" }, HttpStatusCode.NotFound);

            if (Request.Headers.IfModifiedSince.HasValue && Request.Headers.IfModifiedSince >= file.LastWriteTimeUtc.AddSeconds(-1))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotModified,
                    RequestMessage = Request
                };
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var response = new HttpResponseMessage
            {
                Content = new StreamContent(fileStream, (int)file.Length),
                StatusCode = HttpStatusCode.OK,
                RequestMessage = Request
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            response.Content.Headers.LastModified = file.LastWriteTimeUtc;

            return response;
        }
    }
}