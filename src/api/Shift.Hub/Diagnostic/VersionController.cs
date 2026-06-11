using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Diagnostic")]
    public class VersionController : DefaultVersionController
    {
        protected override Version GetVersion()
        {
            return typeof(VersionController).Assembly.GetName().Version
                ?? new Version(0, 0, 0, 0);
        }
    }
}
