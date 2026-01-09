using Engine.Common;

using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Engine.Scorm
{
    [ApiController]
    public class HealthController : DefaultHealthController
    {
        protected override string AppName => "Engine.Integration.Scorm.Api";
        public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
    }
}