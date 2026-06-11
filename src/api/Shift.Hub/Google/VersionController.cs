using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.Google
{
    [ApiController]
    [Route("google")]
    [ApiExplorerSettings(GroupName = "Integration: Google", IgnoreApi = true)]
    public class VersionController : DefaultVersionController
    {
        protected override Version GetVersion()
        {
            return typeof(VersionController).Assembly.GetName().Version
                ?? new Version(0, 0, 0, 0);
        }
    }
}
