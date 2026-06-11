using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Diagnostic")]
    public class HealthController : DefaultHealthController
    {
        protected override string AppName => "Shift.Hub";

        public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
    }
}
