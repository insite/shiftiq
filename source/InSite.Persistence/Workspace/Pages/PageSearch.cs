using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Application.Sites.Read;
using InSite.Domain.Foundations;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class PageSearch : IPageSearch
    {
        #region Caching

        private static void Uncache(IEnumerable<Guid?> sites)
        {
            foreach (var site in sites.Distinct())
                Uncache(site);
        }

        private static void Uncache(Guid? site)
        {
            if (site != null)
                DomainCache.Instance.RemoveSite(site.Value);
        }

        #endregion

        #region Classes

        private class ReadHelper : ReadHelper<QPage>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QPage>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.QPages.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(Expression<Func<QPage, T>> binder, QPageFilter filter)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.QPages.AsQueryable().AsNoTracking();

                    IQueryable<T> bind(IQueryable<QPage> q) => q.Select(binder);

                    IQueryable<QPage> filterQuery(IQueryable<QPage> q) => PageFilterHelper.ApplyFilter(q, filter, context);

                    var modelQuery = BuildQuery(query, bind, filterQuery, q => q, filter.Paging, filter.OrderBy, null, false);

                    return modelQuery.ToArray();
                }
            }

        }

        public static class PageFilterHelper
        {
            internal static IQueryable<QPage> ApplyFilter(IQueryable<QPage> query, QPageFilter filter, InternalDbContext db)
            {
                if (filter.OrganizationIdentifier.HasValue)
                    query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

                if (filter.WebSiteIdentifier.HasValue)
                    query = query.Where(x => x.SiteIdentifier == filter.WebSiteIdentifier.Value);

                if (filter.WebFolderIdentifier.HasValue)
                    query = query.Where(x => x.ParentPageIdentifier == filter.WebFolderIdentifier.Value);

                if (filter.CourseIdentifier.HasValue)
                    query = query.Where(x => x.ObjectType == "Course" && x.ObjectIdentifier == filter.CourseIdentifier);

                if (filter.ProgramIdentifier.HasValue)
                    query = query.Where(x => x.ObjectType == "Program" && x.ObjectIdentifier == filter.ProgramIdentifier);

                if (filter.WebSiteAssigned.HasValue)
                {
                    if (filter.WebSiteAssigned.Value)
                        query = query.Where(x => x.SiteIdentifier.HasValue);
                    else
                        query = query.Where(x => !x.SiteIdentifier.HasValue);
                }

                if (filter.Types.IsNotEmpty())
                    query = query.Where(x => filter.Types.Contains(x.PageType));

                if (filter.Title.IsNotEmpty())
                    query = query.Where(x => x.Title.Contains(filter.Title));

                if (filter.PageSlug.IsNotEmpty())
                    query = query.Where(x => x.PageSlug.Contains(filter.PageSlug));

                if (filter.PageSlugExact.IsNotEmpty())
                    query = query.Where(x => x.PageSlug == filter.PageSlugExact);

                if (filter.Keyword.IsNotEmpty())
                {
                    var expr = LinqExtensions1.Expr((QPage x) => x.PageSlug.Contains(filter.Keyword) || x.Title.Contains(filter.Keyword));
                    query = query.Where(expr.Expand());
                }

                if (filter.IsPublished.HasValue)
                    query = query.Where(x => x.IsHidden == !filter.IsPublished.Value);

                if (filter.PermissionGroupIdentifier.HasValue)
                {
                    query = query.Where(x => db.TGroupPermissions
                        .Where(y => y.ObjectIdentifier == x.PageIdentifier
                            && y.GroupIdentifier == filter.PermissionGroupIdentifier.Value
                        ).Any()
                    );
                }

                if (filter.Modified != null)
                {
                    if (filter.Modified.Since.HasValue)
                        query = query.Where(x => x.LastChangeTime >= filter.Modified.Since.Value);

                    if (filter.Modified.Before.HasValue)
                        query = query.Where(x => x.LastChangeTime < filter.Modified.Before.Value);
                }

                if (filter.ContentControl.IsNotEmpty())
                    query = query.Where(x => x.ContentControl.Contains(filter.ContentControl));

                return query;
            }
        }

        #endregion

        private readonly IChangeRepository _repository;
        private readonly IContentSearch _contentSearch;

        public PageSearch(IChangeRepository repo, IContentSearch contentSearch)
        {
            _repository = repo;
            _contentSearch = contentSearch;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public QPage GetPage(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.QPages
                    .AsNoTracking()
                    .FirstOrDefault(x => x.PageIdentifier == id);
            }
        }

        #region Select (entity)

        public QPage[] Select(Expression<Func<QPage, bool>> filter, params Expression<Func<QPage, object>>[] includes)
            => ReadHelper.Instance.Select(filter, includes);

        public QPage Select(Guid id, params Expression<Func<QPage, object>>[] includes)
            => ReadHelper.Instance.SelectFirst(x => x.PageIdentifier == id, includes);

        public T BindFirst<T>(
            Expression<Func<QPage, T>> binder,
            Expression<Func<QPage, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public T[] Bind<T>(
            Expression<Func<QPage, T>> binder,
            Expression<Func<QPage, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public bool Exists(Expression<Func<QPage, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        public List<Tuple<string, string>> GetCourseWebPages(Guid course)
        {
            var list = new List<Tuple<string, string>>();
            using (var db = new InternalDbContext(false))
            {
                var pages = db.QPages.Where(x => x.ObjectType == "Course" && x.ObjectIdentifier == course);
                if (pages.Count() > 0)
                {
                    foreach (var page in pages)
                    {
                        var path = GetPagePath(page.PageIdentifier, false);
                        var edit = $"/ui/admin/sites/pages/outline?id={page.PageIdentifier}";
                        list.Add(new Tuple<string, string>(path, edit));
                    }
                }
            }
            return list;
        }

        public string GetPagePath(Guid id, bool includeHostName)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<string>(
                    "SELECT sites.GetQPagePath(@PageIdentifier, @IncludeHost)"
                    , new SqlParameter("@PageIdentifier", id)
                    , new SqlParameter("@IncludeHost", includeHostName)
                    ).FirstOrDefault();
            }
        }

        public QPage[] GetSitePages(Guid site)
        {
            using (var db = new InternalDbContext(false))
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                return db.Database
                    .SqlQuery<QPage>("EXEC sites.GetQSitePages @SiteIdentifier", new SqlParameter("@SiteIdentifier", site))
                    .ToArray();
            }
        }

        public QPage[] GetTreePages(Guid page)
        {
            using (var db = new InternalDbContext(false))
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                return db.Database
                    .SqlQuery<QPage>("EXEC sites.GetQTreePages @WebSitePage", new SqlParameter("@WebSitePage", page))
                    .ToArray();
            }
        }

        public QPage[] GetDownstreamPages(Guid page)
        {
            using (var db = new InternalDbContext(false))
                return db.Database
                    .SqlQuery<QPage>("EXEC sites.GetQDownstreamPages @PageIdentifier", new SqlParameter("@PageIdentifier", page))
                    .ToArray();
        }

        public static List<HelpPageRecord> GetOrgHelpPages(Guid organizationId, string keyword)
        {
            const string query = "exec sites.GetOrgHelpPages @OrganizationId, @Keyword";

            using (var db = new InternalDbContext(false))
            {
                return db.Database
                    .SqlQuery<HelpPageRecord>(query, new SqlParameter("OrganizationId", organizationId), new SqlParameter("Keyword", keyword))
                    .ToList();
            }
        }

        public static List<HelpPageRecord> GetHelpPages(string keyword)
        {
            const string query = "exec sites.GetHelpPages @Keyword";

            using (var db = new InternalDbContext(false))
            {
                return db.Database
                    .SqlQuery<HelpPageRecord>(query, new SqlParameter("Keyword", keyword))
                    .ToList();
            }
        }

        #endregion

        #region Select (filter)

        public PageSearchItem[] GetPageSearchItems(QPageFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return PageFilterHelper.ApplyFilter(db.QPages.AsQueryable().AsNoTracking(), filter, db)
                    .GroupJoin(db.TGroupPermissions,
                        page => page.PageIdentifier,
                        permission => permission.ObjectIdentifier,
                        (page, permissions) => new
                        {
                            page.PageIdentifier,
                            page.ParentPageIdentifier,
                            page.PageType,
                            page.PageSlug,
                            page.Hook,
                            PageTitle = page.Title,
                            page.ContentControl,
                            ChildrenCount = page.Children.Count,
                            page.SiteIdentifier,
                            page.Site.SiteTitle,
                            page.Site.SiteDomain,
                            page.IsHidden,
                            GroupPermissions = permissions.Select(x => x.Group.GroupName),
                            page.LastChangeTime,
                            page.LastChangeUser
                        }
                    )
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter.Paging)
                    .ToList()
                    .Select(x => new PageSearchItem
                    {
                        PageIdentifier = x.PageIdentifier,
                        ParentPageIdentifier = x.ParentPageIdentifier,
                        PageType = x.PageType,
                        PageSlug = x.PageSlug,
                        PageHook = x.Hook,
                        PageTitle = x.PageTitle ?? "(Untitled)",
                        ContentControl = x.ContentControl,
                        Scope = x.ParentPageIdentifier == null ? "Root" : (x.ChildrenCount > 0 ? "Branch" : "Leaf"),
                        ChildrenCount = x.ChildrenCount,
                        SiteIdentifier = x.SiteIdentifier,
                        SiteTitle = x.SiteTitle,
                        SiteName = x.SiteDomain,
                        PublicationStatus = !x.IsHidden ? "Published" : "Unpublished",
                        GroupPermissions = x.GroupPermissions.Any()
                            ? string.Join(", ", x.GroupPermissions.OrderBy(y => y))
                            : "(public to all)",
                        LastChangeTime = x.LastChangeTime,
                        LastChangeUser = x.LastChangeUser
                    })
                    .ToArray();
            }
        }

        public int Count(QPageFilter filter)
        {
            using (var db = new InternalDbContext())
                return PageFilterHelper.ApplyFilter(db.QPages.AsQueryable().AsNoTracking(), filter, db).Count();
        }

        public int Count(Expression<Func<QPage, bool>> filter)
            => ReadHelper.Instance.Count(filter);

        public T[] Bind<T>(Expression<Func<QPage, T>> binder, QPageFilter filter)
        {
            return ReadHelper.Instance.Bind(binder, filter);
        }


        #endregion

        public List<Guid> GetPageChildrenIds(Guid page)
        {
            var handles = new List<Guid>();

            using (var db = new InternalDbContext())
            {
                TraverseTree(db, page, pages => { handles.AddRange(pages.Select(x => x.PageIdentifier)); });
            }

            return handles;
        }

        private HashSet<Guid> TraverseTree(InternalDbContext db, Guid resourceId, Action<IEnumerable<QPage>> action)
        {
            var allIds = new HashSet<Guid>();

            var entities = db.QPages.Where(x => x.PageIdentifier == resourceId).ToArray();

            while (entities.Length > 0)
            {
                action(entities);

                var filter = entities.Where(x => allIds.Add(x.PageIdentifier)).Select(x => x.PageIdentifier).ToArray();

                entities = db.QPages.Where(x => filter.Contains(x.ParentPageIdentifier.Value)).ToArray();
            }

            return allIds;
        }

        public List<QPage> GetReorderByResourceId(Guid resourceId, IEnumerable<Guid> data)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.QPages.AsQueryable().Where(x => x.ParentPageIdentifier == resourceId);
                Uncache(query.Select(x => x.SiteIdentifier));
                return Reorder(query, data);
            }
        }

        public List<QPage> GetReorderBySiteId(Guid siteId, IEnumerable<Guid> data)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.QPages.AsQueryable().Where(x => x.SiteIdentifier == siteId && !x.ParentPageIdentifier.HasValue);
                Uncache(query.Select(x => x.SiteIdentifier));
                return Reorder(query, data);
            }
        }

        private List<QPage> Reorder(IQueryable<QPage> query, IEnumerable<Guid> data)
        {
            if (data == null)
                return null;

            var resources = query.ToDictionary(x => x.PageIdentifier);
            if (resources.Count == 0)
                return null;

            var ordered = new List<QPage>();
            foreach (var id in data)
            {
                if (!resources.TryGetValue(id, out var resource))
                    continue;

                ordered.Add(resource);
                resources.Remove(id);
            }

            foreach (var resource in resources.Values.OrderBy(x => x.Sequence).ThenBy(x => x.Title))
                ordered.Add(resource);

            for (var i = 0; i < ordered.Count; i++)
                ordered[i].Sequence = i + 1;

            return ordered;
        }

        #region Page Trees

        public PageTree CreateTree(Guid site)
        {
            return PageTreeBuilder.BuildTree(GetPageNodes(site));
        }

        private PageNode[] GetPageNodes(Guid site)
        {
            var pages = Select(x => x.SiteIdentifier == site, x => x.Parent, x => x.Children)
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Title)
                .ToArray();

            var list = new List<PageNode>();
            foreach (var page in pages)
                list.Add(CreatePageNode(page));

            if (list.Count(x => x.Parent == null) > 1)
            {
                list.Add(new PageNode { Identifier = Guid.Empty, Name = "(Root)" });
                foreach (var item in list.Where(x => x.Identifier != Guid.Empty && x.Parent == null))
                    item.Parent = Guid.Empty;
            }

            return list.ToArray();
        }

        private PageNode CreatePageNode(QPage item)
        {
            var node = new PageNode
            {
                Author = item.AuthorName,
                Authored = item.AuthorDate,
                Control = item.ContentControl,
                Icon = item.PageIcon,
                Identifier = item.PageIdentifier,
                IsHidden = item.IsHidden,
                Modified = item.LastChangeTime.Value,
                Name = item.Title,
                NavigateUrl = item.NavigateUrl,
                Parent = item.ParentPageIdentifier,
                Sequence = item.Sequence,
                Slug = item.PageSlug,
                Type = item.PageType
            };
            return node;
        }

        #endregion

        #region Page Serialization

        #region Serialize

        public byte[] SerializeSite(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var site = db.QSites.AsQueryable().Where(x => x.SiteIdentifier == id).FirstOrDefault();
                var webSite = ReadWebSite(site);

                var json = JsonConvert.SerializeObject(webSite, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

                return Encoding.UTF8.GetBytes(json);
            }
        }

        private QSiteExport ReadWebSite(QSite site)
        {
            var content = _contentSearch.GetBlock(site.SiteIdentifier);

            var serialized = new QSiteExport
            {
                Name = site.SiteDomain,
                Title = site.SiteTitle,
                Content = content
            };

            var pages = Bind(
                x => x,
                x => x.SiteIdentifier == site.SiteIdentifier && x.ParentPageIdentifier == null,
                "Sequence");

            if (pages.Length > 0)
            {
                serialized.Pages = new List<QPageExport>();

                foreach (var page in pages)
                {
                    var serializedPage = ReadWebPage(page, site.SiteDomain);
                    serialized.Pages.Add(serializedPage);
                }
            }

            return serialized;
        }

        public byte[] SerializePage(Guid id)
        {
            var page = Select(id, x => x.Site);
            var webPage = ReadWebPage(page, page.Site?.SiteDomain);

            var json = JsonConvert.SerializeObject(webPage, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            });

            return Encoding.UTF8.GetBytes(json);
        }

        private QPageExport ReadWebPage(QPage page, string webSite)
        {
            var content = _contentSearch.GetBlock(page.PageIdentifier);
            var groups = TGroupPermissionSearch.Select(x => x.ObjectIdentifier == page.PageIdentifier, null, x => x.Group);

            var serialized = new QPageExport
            {
                PageType = page.PageType,
                Title = page.Title,
                IsHidden = page.IsHidden,
                PageSlug = page.PageSlug,
                NavigateUrl = page.NavigateUrl,
                IsNewTab = page.IsNewTab,
                Site = webSite,
                Icon = page.PageIcon,
                ContentLabels = page.ContentLabels,
                ContentControl = page.ContentControl,
                Hook = page.Hook,
                Groups = groups.Select(x => x.Group.GroupName).OrderBy(x => x).ToArray(),
                Content = content
            };

            var children = Select(page.PageIdentifier, x => x.Children)
                .Children
                .OrderBy(x => x.Sequence)
                .ToList();

            if (children.Count > 0)
            {
                serialized.Children = new List<QPageExport>();

                foreach (var child in children)
                {
                    var serializedChild = ReadWebPage(child, webSite);
                    serialized.Children.Add(serializedChild);
                }
            }

            return serialized;
        }

        #endregion

        #region Deserialize

        public void LoadSite(Guid? parentOrganization, Guid organization, Guid user, QSiteExport exportSite, QSite site)
        {
            if (exportSite.Pages.IsNotEmpty())
            {
                var pages = site.Pages.ToArray();
                for (int i = 0; i < exportSite.Pages.Count; i++)
                    LoadPage(parentOrganization, organization, user, exportSite.Pages[i], pages[i], site.SiteIdentifier);
            }
        }

        private Guid? GetGroupId(string name, Guid organization, Guid? parentOrganization)
        {
            var organizations = new List<Guid> { organization };
            if (parentOrganization.HasValue)
                organizations.Add(parentOrganization.Value);

            using (var db = new InternalDbContext())
            {
                return db.QGroups
                    .Where(x => organizations.Contains(organization) && x.GroupName == name)
                    .FirstOrDefault()?
                    .GroupIdentifier;
            }
        }

        public void LoadPage(Guid? parentOrganization, Guid organization, Guid user, QPageExport exportPage, QPage page, Guid? webSiteIdentifier, Dictionary<string, Guid?> groups = null)
        {
            if (page.ParentPageIdentifier == null)
            {
                if (webSiteIdentifier == null && exportPage.Site.IsNotEmpty())
                    webSiteIdentifier = SiteSearch.SelectFirst(x => x.OrganizationIdentifier == organization && x.SiteDomain == exportPage.Site)?.SiteIdentifier;

                groups = new Dictionary<string, Guid?>(StringComparer.OrdinalIgnoreCase);
            }

            page.IsHidden = exportPage.IsHidden;
            page.PageSlug = exportPage.PageSlug;
            page.NavigateUrl = exportPage.NavigateUrl;
            page.IsNewTab = exportPage.IsNewTab;
            page.PageIcon = exportPage.Icon;
            page.ContentLabels = exportPage.ContentLabels;
            page.ContentControl = exportPage.ContentControl;
            page.Hook = exportPage.Hook;
            page.SiteIdentifier = webSiteIdentifier;

            if (exportPage.Groups.IsNotEmpty())
            {
                foreach (var groupName in exportPage.Groups)
                {
                    if (!groups.TryGetValue(groupName, out var groupIdentifier))
                    {
                        groupIdentifier = GetGroupId(groupName, organization, parentOrganization);

                        groups.Add(groupName, groupIdentifier);
                    }
                }
            }

            if (exportPage.Children.IsNotEmpty())
            {
                var pageChildren = page.Children.ToArray();
                for (int i = 0; i < exportPage.Children.Count; i++)
                    LoadPage(parentOrganization, organization, user, exportPage.Children[i], pageChildren[i], webSiteIdentifier, groups);
            }
        }

        public void SavePageContent(QPageExport exportPage, QPage page)
        {
            if (exportPage.Content != null && !exportPage.Content.IsEmpty)
                new TContentStore().SaveContainer(page.OrganizationIdentifier, ContentContainerType.WebPage, page.PageIdentifier, exportPage.Content);

            if (exportPage.Children.IsNotEmpty())
            {
                var pageChildren = page.Children.ToArray();
                for (int i = 0; i < exportPage.Children.Count; i++)
                    SavePageContent(exportPage.Children[i], pageChildren[i]);
            }
        }

        #endregion

        #endregion

        #region Assessment Pages

        public int Count(VAssessmentPageFilter filter)
        {
            using (var db = new InternalDbContext())
                return BuildQuery(db, filter).Count();
        }

        public VAssessmentPage[] Select(VAssessmentPageFilter filter)
        {
            using (var db = new InternalDbContext())
                return BuildQuery(db, filter)
                    .OrderBy(x => x.FormName)
                    .ApplyPaging(filter)
                    .ToArray();
        }

        public VAssessmentPage GetAssessmentPage(Guid pageId)
        {
            var filter = new VAssessmentPageFilter { PageIdentifier = pageId };
            using (var db = new InternalDbContext())
                return BuildQuery(db, filter).FirstOrDefault();
        }

        private IQueryable<VAssessmentPage> BuildQuery(InternalDbContext db, VAssessmentPageFilter filter)
        {
            var query = db.VAssessmentPages.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.PageTitle))
                query = query.Where(x => x.PageTitle.Contains(filter.PageTitle));

            if (!string.IsNullOrEmpty(filter.FormName))
                query = query.Where(x => x.FormName.Contains(filter.FormName));

            if (filter.FormAsset != null)
                query = query.Where(x => x.FormAsset == filter.FormAsset);

            if (filter.PageIdentifier != null)
                query = query.Where(x => x.PageIdentifier == filter.PageIdentifier);

            if (filter.PageIsHidden != null)
                query = query.Where(x => x.PageIsHidden == filter.PageIsHidden);

            return query;
        }

        private class AssessmentPageHelper : ReadHelper<VAssessmentPage>
        {
            public static readonly AssessmentPageHelper Instance = new AssessmentPageHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VAssessmentPage>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VAssessmentPages.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public VAssessmentPage[] GetAssessmentPages(Guid[] formIdentifiers)
        {
            using (var db = new InternalDbContext())
                return db.VAssessmentPages.Where(x => formIdentifiers.Contains(x.FormIdentifier)).ToArray();
        }

        #endregion
    }
}
