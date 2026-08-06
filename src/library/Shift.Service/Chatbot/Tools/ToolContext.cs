using Shift.Common;

namespace Shift.Service.Chatbot.Tools;

public class ToolContext
{
    public PermissionCache PermissionCache { get; }
    public IPrincipal Principal { get; }
    public string[] Roles { get; }

    public ToolContext(PermissionCache permissionCache, IPrincipal principal)
    {
        PermissionCache = permissionCache;
        Principal = principal;
        Roles = [..principal.Roles.Select(x => x.Name)];
    }
}