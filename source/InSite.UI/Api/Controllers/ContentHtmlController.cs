using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Web;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Content")]
    [RoutePrefix("api/contents")]
    public partial class ContentsController : ApiBaseController
    {
        [HttpGet]
        [Route("html")]
        public HttpResponseMessage Html()
        {
            var request = HttpContext.Current.Request;

            var lang = request["lang"].IfNullOrEmpty("en");
            var type = request["type"].IfNullOrEmpty("none");

            if (!Guid.TryParse(request["container"], out var containerId))
                containerId = Guid.Empty;

            var result = new List<HtmlResultItem>();
            var content = ServiceLocator.ContentSearch.GetBlock(containerId);

            var keys = type == "standard"
                ? CurrentOrganization.GetStandardContentLabels()
                : new string[0];

            if (keys.Length == 0)
                keys = content.GetLabels().OrderBy(x => x).ToArray();

            foreach (var k in keys)
            {
                var html = content.GetHtml(k, lang);
                if (html.IsNotEmpty())
                    result.Add(new HtmlResultItem
                    {
                        Title = k,
                        Value = html
                    });
            }

            return JsonSuccess(result);
        }

        [HttpPost]
        [Route("markdown-to-html")]
        public HttpResponseMessage MarkdownToHtml([FromBody] FormDataCollection formData)
        {
            var markdown = formData["markdown"];
            if (string.IsNullOrEmpty(markdown))
                return JsonSuccess("No Content", HttpStatusCode.NoContent);

            var html = Markdown.ToHtml(markdown);

            return JsonSuccess(html);
        }

        [HttpGet]
        [Route("proxy")]
        public async Task<HttpResponseMessage> Proxy(string url)
        {
            try
            {
                return await StaticHttpClient.Client.GetAsync(url);
            }
            catch (InvalidOperationException ex)
            {
                return ServerErrorActionResult.CreateResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }
    }
}