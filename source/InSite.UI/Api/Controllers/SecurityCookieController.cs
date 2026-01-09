using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using InSite.Api.Settings;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Security")]
    [RoutePrefix("api/client")]
    [ApiAuthenticationRequirement(ApiAuthenticationType.Cookie)]
    public class ClientController : ApiBaseController
    {
        public static string ClientUrl = "/api/client";

        [HttpPost]
        [Route("cookie")]
        public HttpResponseMessage GetCookieToken()
        {
            if (ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Local)
                return BadRequest("An authentication cookie can be generated only in the Local environment for use on the localhost domain.");

            return JsonSuccess(CookieTokenModule.Current, false);
        }

        new private HttpResponseMessage BadRequest(string text)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(text),
                StatusCode = HttpStatusCode.BadRequest,
                RequestMessage = Request
            };

            return response;
        }

        private HttpResponseMessage JsonSuccess(object data, bool camelCaseNames = true)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            if (camelCaseNames)
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var json = JsonConvert.SerializeObject(data, settings);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
    }
}