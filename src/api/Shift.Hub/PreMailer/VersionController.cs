using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.PreMailer
{
    [ApiController]
    [Route("premailer")]
    [ApiExplorerSettings(GroupName = "Integration: PreMailer", IgnoreApi = true)]
    public class VersionController : DefaultVersionController
    {
        protected override Version GetVersion()
        {
            return typeof(VersionController).Assembly.GetName().Version
                ?? new Version(0, 0, 0, 0);
        }
    }
}
