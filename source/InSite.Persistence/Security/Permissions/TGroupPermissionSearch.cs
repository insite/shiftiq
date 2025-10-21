using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Application.Sites.Read;
using InSite.Domain.Foundations;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class TGroupPermissionSearch
    {
        private class ReadHelper : ReadHelper<TGroupPermission>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TGroupPermission>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TGroupPermissions.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static bool AllowImpersonation(GroupList groups)
        {
            using (var context = new InternalDbContext())
            {
                context.Configuration.ProxyCreationEnabled = false;

                var actionId = context.TActions
                    .Where(x => x.ActionUrl == "ui/portal/identity/impersonate")
                    .Select(x => x.ActionIdentifier)
                    .FirstOrDefault();

                var authorizations = context
                    .TGroupPermissions
                    .AsQueryable()
                    .AsNoTracking()
                    .Where(x => x.ObjectIdentifier == actionId)
                    .ToList();

                var keys = groups
                    .Select(x => x.Identifier)
                    .ToList();

                return authorizations.Any(x => keys.Contains(x.GroupIdentifier));
            }
        }

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<TGroupPermission, T>> binder,
            Expression<Func<TGroupPermission, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static int Count(TGroupActionFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Select(x => new
                    {
                        x.GroupIdentifier,
                        x.ObjectIdentifier
                    })
                    .Distinct()
                    .Count();
            }
        }

        private static IQueryable<TGroupPermission> CreateQuery(TGroupActionFilter filter, InternalDbContext db)
        {
            var query = db.TGroupPermissions.AsQueryable();

            if (filter == null)
                return query;

            if (filter.ActionIdentifier.HasValue)
                query = query.Where(x => x.ObjectIdentifier == filter.ActionIdentifier);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.GroupIdentifier == filter.GroupIdentifier);

            if (!string.IsNullOrEmpty(filter.GroupType))
                query = query.Where(x => x.Group.GroupType == filter.GroupType);

            if (filter.AllowRead.HasValue)
                query = query.Where(x => x.AllowRead == filter.AllowRead);

            if (filter.AllowWrite.HasValue)
                query = query.Where(x => x.AllowWrite == filter.AllowWrite);

            if (filter.AllowDelete.HasValue)
                query = query.Where(x => x.AllowDelete == filter.AllowDelete);

            if (filter.AllowFullControl.HasValue)
                query = query.Where(x => x.AllowConfigure == filter.AllowFullControl);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Group.OrganizationIdentifier == filter.OrganizationIdentifier);

            return query;
        }

        public static List<TGroupPermission> SelectByGroup(Guid group)
        {
            using (var context = new InternalDbContext())
            {
                return context.TGroupPermissions
                    .Where(x => x.GroupIdentifier == group)
                    .ToList();
            }
        }

        public static Guid[] SelectGroupFromActionPermission(string actionUrl)
        {
            using (var db = new InternalDbContext())
            {
                return db.TGroupPermissions.Join(db.TActions.Where(x => x.ActionUrl == actionUrl),
                        a => a.ObjectIdentifier,
                        b => b.ActionIdentifier,
                        (a, b) => a.GroupIdentifier
                    )
                    .ToArray();
            }
        }

        public static TGroupPermission SelectFirst(
            Expression<Func<TGroupPermission, bool>> filter,
            params Expression<Func<TGroupPermission, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, includes);

        public static void SetIsAccessDenied(QPage page, ISecurityFramework identity, List<TGroupPermission> permissions)
        {
            page.IsAccessDenied = false;

            var pagePermissions = permissions.Where(x => x.ObjectIdentifier == page.PageIdentifier).Select(x => x.GroupIdentifier).ToList();
            if (pagePermissions.Count > 0)
            {
                var grants = identity.Groups.Select(x => x.Identifier).ToList();
                page.IsAccessDenied = !pagePermissions.Intersect(grants).Any();
            }

            if (page.Children != null)
                SetIsAccessDenied(page.Children.ToList(), identity, permissions);
        }

        public static void SetIsAccessDenied(List<QPage> pages, ISecurityFramework identity, List<TGroupPermission> permissions)
        {
            var grants = identity.Groups.Select(x => x.Identifier).ToList();
            var pageIdentifiers = pages.Select(x => x.PageIdentifier).ToList();

            foreach (var page in pages)
            {
                var pagePermissions = permissions.Where(x => x.ObjectIdentifier == page.PageIdentifier).Select(x => x.GroupIdentifier).ToList();
                if (pagePermissions.Count > 0)
                    page.IsAccessDenied = pagePermissions.Intersect(grants).Count() == 0;
            }
        }

        public static HashSet<Guid> GetAccessAllowed(IEnumerable<Guid> objectIds, ISecurityFramework identity, IEnumerable<TGroupPermission> permissions)
        {
            var grants = identity.Groups.Select(x => x.Identifier).ToList();
            var result = new HashSet<Guid>();

            foreach (var objectId in objectIds)
            {
                var objectPermissions = permissions.Where(x => x.ObjectIdentifier == objectId).Select(x => x.GroupIdentifier).ToList();
                var isAccessDenied = objectPermissions.Count > 0 && objectPermissions.Intersect(grants).Count() == 0;

                if (!isAccessDenied)
                    result.Add(objectId);
            }

            return result;
        }

        public static HashSet<Guid> GetAccessAllowed(IEnumerable<Guid> objectIds, ISecurityFramework identity)
        {
            var organizationId = identity.Organization.Identifier;

            var permissions = Select(p =>
                p.OrganizationIdentifier == organizationId
                && objectIds.Any(id => p.ObjectIdentifier == id)
            );

            return GetAccessAllowed(objectIds, identity, permissions);
        }

        public static bool IsAccessDenied(Guid objectId, ISecurityFramework identity)
        {
            var permissions = ReadHelper.Instance.Bind(x => x.GroupIdentifier, x => x.ObjectIdentifier == objectId);

            return permissions.Length > 0
                && !identity.Groups.Any(x => permissions.Contains(x.Identifier));
        }

        public static bool IsAccessAllowed(Guid objectId, ISecurityFramework identity)
        {
            var permissions = ReadHelper.Instance.Bind(x => x.GroupIdentifier, x => x.ObjectIdentifier == objectId);

            return permissions.Length == 0
                || identity.Groups.Any(x => permissions.Contains(x.Identifier));
        }

        public static bool Exists(Expression<Func<TGroupPermission, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        public static List<Guid> GetObjectsAccessibleToUser(Guid user)
        {
            return TGroupPermissionSearch.Bind(
                x => x.ObjectIdentifier,
                x => x.Group.VMemberships.Any(y => y.UserIdentifier == user)
            )
                .Distinct()
                .ToList();
        }

        public static TGroupPermission Select(Guid permission)
        {
            using (var context = new InternalDbContext())
            {
                return context.TGroupPermissions
                    .FirstOrDefault(x => x.PermissionIdentifier == permission);
            }
        }

        public static TGroupPermission Select(Guid group, Guid action)
        {
            using (var context = new InternalDbContext())
            {
                return context.TGroupPermissions
                    .FirstOrDefault(x => x.GroupIdentifier == group && x.ObjectIdentifier == action);
            }
        }

        public static SearchResultList Select(TGroupActionFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var list = CreateQuery(filter, db)
                    .Select(x => new TGroupPermissionSearchResult
                    {
                        PermissionIdentifier = x.PermissionIdentifier,

                        ObjectIdentifier = x.ObjectIdentifier,
                        ObjectType = x.ObjectType,

                        GroupIdentifier = x.GroupIdentifier,
                        GroupName = x.Group.GroupName,
                        GroupType = x.Group.GroupType,

                        OrganizationCode = x.Group.Organization.OrganizationCode,
                        OrganizationIdentifier = x.Group.OrganizationIdentifier,

                        AllowExecute = x.AllowExecute,

                        AllowRead = x.AllowRead,
                        AllowWrite = x.AllowWrite,
                        AllowCreate = x.AllowCreate,
                        AllowDelete = x.AllowDelete,
                        AllowAdministrate = x.AllowAdministrate,
                        AllowConfigure = x.AllowConfigure,
                        AllowTrialAccess = x.AllowTrialAccess,

                        Allow = x.AllowExecute || x.AllowRead || x.AllowWrite || x.AllowDelete || x.AllowCreate || x.AllowAdministrate || x.AllowConfigure
                    })
                    .ToList();

                foreach (TGroupPermissionSearchResult item in list)
                {
                    var action = TActionSearch.Get(item.ObjectIdentifier);
                    if (action != null)
                    {
                        item.ObjectName = action.ActionName;
                        item.ObjectSubtype = action.ActionType;
                    }
                    var page = (new PageSearch(null, null)).Select(item.ObjectIdentifier);
                    if (page != null)
                    {
                        item.ObjectName = page.PageSlug;
                        item.ObjectSubtype = page.PageType;
                    }
                }

                return list
                    .OrderBy(x => x.GroupName)
                    .ThenBy(x => x.ObjectType)
                    .ThenBy(x => x.ObjectSubtype)
                    .ThenBy(x => x.ObjectName)
                    .AsQueryable()
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static TGroupPermission[] Select(Expression<Func<TGroupPermission, bool>> filter, string sortExpression = null, params Expression<Func<TGroupPermission, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T[] Distinct<T>(Expression<Func<TGroupPermission, T>> binder, Expression<Func<TGroupPermission, bool>> filter) =>
            ReadHelper.Instance.Distinct(binder, filter, null);

        public static TGroupPermission[] GetByObjectId(Guid objectId, params Expression<Func<TGroupPermission, object>>[] includes)
        {
            using (var db = new InternalDbContext())
            {
                return db.TGroupPermissions
                    .ApplyIncludes(includes)
                    .Where(x => x.ObjectIdentifier == objectId)
                    .ToArray();
            }
        }
    }
}
