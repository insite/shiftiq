using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Engine.Common
{
    public abstract class DefaultVersionController : ControllerBase
    {
        protected abstract Version GetVersion();

        [HttpGet("version")]
        [ProducesResponseType<VersionResult>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("apiVersion")]
        public IActionResult Version()
        {
            var version = GetVersion();

            var body = new VersionResult
            {
                Version = version,
                Major = version.Major,
                Minor = version.Minor,
                Build = version.Build,
                Revision = version.Revision
            };

            return Ok(body);
        }
    }
}
