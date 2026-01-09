using Engine.Common;

using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Engine.CssToHtml;

[ApiController]
public class HealthController : DefaultHealthController
{
    protected override string AppName => "Engine.Integration.PreMailer.Api";
    public HealthController(ReleaseSettings releaseSettings, IMonitor monitor) : base(releaseSettings, monitor) { }
}