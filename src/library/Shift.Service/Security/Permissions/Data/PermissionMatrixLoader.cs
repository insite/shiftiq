using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Service
{
    /// <summary>
    /// .NET 9 / Entity Framework Core implementation of the permission matrix loader.
    /// </summary>
    public class PermissionMatrixLoader : PermissionMatrixLoaderBase
    {
        private readonly IDbContextFactory<TableDbContext> _contextFactory;
        private readonly RouteSettings _routeSettings;

        public PermissionMatrixLoader(IDbContextFactory<TableDbContext> contextFactory, RouteSettings routeSettings)
        {
            _contextFactory = contextFactory;
            _routeSettings = routeSettings;
        }

        protected override IReadOnlyList<RouteEndpoint> GetRoutes()
        {
            using var db = _contextFactory.CreateDbContext();
            return db.RouteEndpoint.ToList();
        }

        protected override IReadOnlyList<OrganizationInfo> GetActiveOrganizations()
        {
            using var db = _contextFactory.CreateDbContext();
            return db.Organization
                .AsNoTracking()
                .Where(x => x.AccountClosed == null)
                .OrderBy(x => x.OrganizationCode)
                .Select(x => new OrganizationInfo
                {
                    OrganizationIdentifier = x.OrganizationIdentifier,
                    OrganizationCode = x.OrganizationCode
                })
                .ToList();
        }

        protected override IReadOnlyList<(Guid OrganizationId, GroupPermissionInfo Permission)> GetPermissionsGrantedOnActions()
        {
            using var db = _contextFactory.CreateDbContext();

            return db.TPermission
                .AsNoTracking()
                .Include(x => x.Group)
                .Where(x => x.Group != null)
                .Join(db.TAction,
                    x => x.ObjectIdentifier,
                    y => y.ActionIdentifier,
                    (permission, action) => new
                    {
                        OrganizationId = permission.Group!.OrganizationIdentifier,
                        Permission = new GroupPermissionInfo
                        {
                            Resource = action.ActionUrl,
                            Role = permission.Group.GroupName,
                            AllowRead = permission.AllowRead,
                            AllowWrite = permission.AllowWrite,
                            AllowCreate = permission.AllowCreate,
                            AllowDelete = permission.AllowDelete,
                            AllowAdministrate = permission.AllowAdministrate,
                            AllowConfigure = permission.AllowConfigure
                        }
                    })
                .AsEnumerable() // switch to client-side for tuple construction
                .Select(x => (x.OrganizationId, x.Permission))
                .ToList();
        }

        protected override IReadOnlyList<ResourcePermissions> GetExtraPermissionsGranted(Guid organizationId)
        {
            using var db = _contextFactory.CreateDbContext();

            var permissions = db.OrganizationPermission.SingleOrDefault(x => x.OrganizationId == organizationId);

            if (permissions == null || permissions.AccessGranted == null)
                return new List<ResourcePermissions>();

            var list = JsonConvert.DeserializeObject<List<ResourcePermissions>>(permissions.AccessGranted);

            return list ?? new List<ResourcePermissions>();
        }

        protected override IReadOnlyList<ResourcePermissions> GetExtraPermissionsDenied(Guid organizationId)
        {
            using var db = _contextFactory.CreateDbContext();

            var permissions = db.OrganizationPermission.SingleOrDefault(x => x.OrganizationId == organizationId);

            if (permissions == null || permissions.AccessDenied == null)
                return new List<ResourcePermissions>();

            var list = JsonConvert.DeserializeObject<List<ResourcePermissions>>(permissions.AccessDenied);

            return list ?? new List<ResourcePermissions>();
        }

        /// <summary>
        /// Gets the route configuration settings needed for permission inheritance.
        /// </summary>
        protected override RouteSettings GetRouteSettings()
        {
            return _routeSettings;
        }
    }
}