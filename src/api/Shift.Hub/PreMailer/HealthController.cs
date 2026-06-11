using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.PreMailer
{
    [ApiController]
    [Route("premailer")]
    [ApiExplorerSettings(GroupName = "Integration: PreMailer", IgnoreApi = true)]
    public class HealthController : DefaultHealthController
    {
        protected override string AppName => "Engine.Integration.PreMailer.Api";

        public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
    }
}
