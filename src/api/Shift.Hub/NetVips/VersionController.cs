using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.NetVips
{
    [ApiController]
    [Route("imagemagick")]
    [ApiExplorerSettings(GroupName = "Integration: NetVips", IgnoreApi = true)]
    public class VersionController : DefaultVersionController
    {
        protected override Version GetVersion()
        {
            return typeof(VersionController).Assembly.GetName().Version
                ?? new Version(0, 0, 0, 0);
        }
    }
}
