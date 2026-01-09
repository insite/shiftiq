using System.ComponentModel;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;

namespace InSite.Api.Controllers
{
    [DisplayName("Platform")]
    public class HealthController : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/v1/status")]
        public HttpResponseMessage Status()
        {
            var version = AppSentry.AssemblyVersion;

            var environment = ServiceLocator.AppSettings.Release.Environment;

            return StringSuccess($"Shift API (v1) release {version} is online. The {environment} environment says hello.");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/v1/version")]
        public HttpResponseMessage Version()
        {
            var version = AppSentry.AssemblyVersion;

            return StringSuccess(version);
        }
    }
}