using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Shift.Common;

namespace Engine.Common
{
    public abstract class DefaultHealthController : ControllerBase
    {
        protected abstract string AppName { get; }

        private readonly ReleaseSettings _releaseSettings;
        private readonly IMonitor _monitor;

        public DefaultHealthController(ReleaseSettings releaseSettings, IMonitor monitor)
        {
            _releaseSettings = releaseSettings;
            _monitor = monitor;
        }

        [HttpGet("status")]
        [ProducesResponseType<ApiStatusResult>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("apiStatus_status")]
        public IActionResult Status() => GetApiStatus();

        [HttpGet("health")]
        [ProducesResponseType<ApiStatusResult>(StatusCodes.Status200OK, "application/json")]
        [EndpointName("apiStatus_health")]
        public IActionResult Health() => GetApiStatus();

        private IActionResult GetApiStatus()
        {
            var environment = _releaseSettings.GetEnvironment();
            var version = _releaseSettings.Version;
            var status = $"{AppName} version {version} is online. The {environment} environment says hello.";

            ApiStatusResult model = environment.IsLocal() && _releaseSettings.ConfigurationProviders?.Any() == true
                ? new ApiConfigurationResult
                {
                    Status = status,
                    Version = version,
                    Environment = environment,
                    Configuration = new { Providers = _releaseSettings.ConfigurationProviders }
                }
                : new ApiStatusResult
                {
                    Status = status,
                    Version = version,
                    Environment = environment
                };

            return Ok(model);
        }

        [HttpPost("error")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("throwException")]
        public ActionResult<string> ThrowError()
        {
            try
            {
                throw new Exception("Error! This is a test exception.");
            }
            catch (Exception ex)
            {
                _monitor.Error(ex.Message);

                return StatusCode(500, ex.Message);
            }
        }
    }
}
