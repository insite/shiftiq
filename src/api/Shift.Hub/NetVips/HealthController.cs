using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.NetVips
{
    [ApiController]
    [Route("imagemagick")]
    [ApiExplorerSettings(GroupName = "Integration: NetVips", IgnoreApi = true)]
    public class HealthController : DefaultHealthController
    {
        protected override string AppName => "Engine.Integration.ImageMagick.Api";

        public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
    }
}
