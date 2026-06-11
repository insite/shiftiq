using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub
{
    public abstract class DefaultVersionController : ControllerBase
    {
        protected virtual Version GetVersion()
        {
            return typeof(DefaultVersionController).Assembly.GetName().Version
                ?? new Version(0, 0, 0, 0);
        }

        [HttpGet("version")]
        [ProducesResponseType<VersionResult>(StatusCodes.Status200OK, "application/json")]
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
