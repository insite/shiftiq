using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.Google
{
    [ApiController]
    [Route("google")]
    [ApiExplorerSettings(GroupName = "Integration: Google", IgnoreApi = true)]
    public class HealthController : DefaultHealthController
    {
        protected override string AppName => "Engine.Integration.Google.Api";

        public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
    }
}
