using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.ScormCloud
{
    [ApiController]
    [Route("scorm")]
    [ApiExplorerSettings(GroupName = "Integration: SCORM Cloud", IgnoreApi = true)]
    public class HealthController : DefaultHealthController
    {
        protected override string AppName => "Engine.Integration.Scorm.Api";

        public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
    }
}
