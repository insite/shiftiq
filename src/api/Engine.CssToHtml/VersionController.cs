using Engine.Common;

using Microsoft.AspNetCore.Mvc;

namespace Engine.CssToHtml;

[ApiController]
public class VersionController : DefaultVersionController
{
    protected override Version GetVersion()
    {
        return typeof(VersionController).Assembly.GetName().Version
            ?? new Version(0, 0, 0, 0);
    }
}
