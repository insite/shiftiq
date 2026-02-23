using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence
{
    public class PermissionMatrixLoader : PermissionMatrixLoaderBase
    {
        private RouteSettings _routeSettings;

        public PermissionMatrixLoader(RouteSettings routeSettings)
        {
            _routeSettings = routeSettings;
        }

        protected override IReadOnlyList<RouteEndpoint> GetRoutes()
        {
            using (var db = new InternalDbContext())
            {
                return db.RouteEndpoints.ToList();
            }
        }

        protected override IReadOnlyList<OrganizationInfo> GetActiveOrganizations()
        {
            using (var db = new InternalDbContext())
            {
                return db.Organizations
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
        }

        protected override IReadOnlyList<(Guid OrganizationId, GroupPermissionInfo Permission)> GetPermissionsGrantedOnActions()
        {
            using (var db = new InternalDbContext())
            {
                return db.TGroupPermissions
                    .AsNoTracking()
                    .Include(x => x.Group)
                    .Where(x => x.Group != null)
                    .Join(db.TActions,
                        x => x.ObjectIdentifier,
                        y => y.ActionIdentifier,
                        (permission, action) => new
                        {
                            OrganizationId = permission.Group.OrganizationIdentifier,
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
        }

        protected override IReadOnlyList<ResourcePermissions> GetExtraPermissionsGranted(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var permissions = db.OrganizationPermissions.SingleOrDefault(x => x.OrganizationId == organizationId);

                if (permissions == null || permissions.AccessGranted == null)
                    return new List<ResourcePermissions>();

                var list = JsonConvert.DeserializeObject<ResourcePermissions[]>(permissions.AccessGranted);

                return list;
            }
        }

        protected override IReadOnlyList<ResourcePermissions> GetExtraPermissionsDenied(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var permissions = db.OrganizationPermissions.SingleOrDefault(x => x.OrganizationId == organizationId);

                if (permissions == null || permissions.AccessDenied == null)
                    return new List<ResourcePermissions>();

                var list = JsonConvert.DeserializeObject<ResourcePermissions[]>(permissions.AccessDenied);

                return list;
            }
        }

        /// <summary>
        /// Gets the subroute configuration for permission inheritance.
        /// </summary>
        protected override RouteSettings GetRouteSettings()
        {
            return _routeSettings;
        }
    }
}