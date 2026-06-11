using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.ScormCloud
{
    [ApiController]
    [Route("scorm")]
    [ApiExplorerSettings(GroupName = "Integration: SCORM Cloud", IgnoreApi = true)]
    public class VersionController : DefaultVersionController
    {
        protected override Version GetVersion()
        {
            return typeof(VersionController).Assembly.GetName().Version
                ?? new Version(0, 0, 0, 0);
        }
    }
}
