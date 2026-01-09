using Engine.Common;

using Microsoft.AspNetCore.Mvc;

namespace Engine.Api;

[ApiController]
public class HealthController : DefaultHealthController
{
    protected override string AppName => "Engine.Integration.Google.Api";
    public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
}
