using Shift.Common;
using Shift.Sdk.UI.Navigation;

namespace Shift.Service.Orchestration
{
    internal class NavigationIdentity : INavigationIdentity
    {
        private readonly PermissionList _permissions;
        private readonly IPrincipal _principal;

        public string PartitionSlug => _principal.Partition.Slug;

        public bool IsAdministrator => _principal.Authority >= AuthorityAccess.Administrator;

        public bool IsOperator => _principal.Authority >= AuthorityAccess.Operator;

        public NavigationIdentity(PermissionCache permissions, IPrincipal principal, bool isCmds)
        {
            _permissions = isCmds
                ? permissions.Matrix.GetPermissions("cmds")
                : permissions.Matrix.GetPermissions(principal.Organization.Slug)
                ;

            _principal = principal;
        }

        public bool IsGranted(string resource)
        {
            return _permissions.IsAllowed(resource, _principal.Roles);
        }

        public bool IsInRole(string role)
        {
            return _principal.Roles.Any(x => string.Equals(x.Name, role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
