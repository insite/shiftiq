using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Contents.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Sites.Utilities
{
    public static class SiteHelper
    {
        #region Classes

        [Serializable]
        public class QSiteTree
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid SiteIdentifier { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }

            public ContentContainer Content { get; set; }
            public List<QPageNode> Pages { get; set; }
        }

        [Serializable]
        public class QPageNode
        {
            public Guid? CourseIdentifier { get; set; }
            public Guid? SurveyIdentifier { get; set; }
            public Guid? ParentPageIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public Guid PageIdentifier { get; set; }
            public Guid? SiteIdentifier { get; set; }

            public string AuthorName { get; set; }
            public string ContentControl { get; set; }
            public string ContentLabels { get; set; }
            public string Hook { get; set; }
            public string NavigateUrl { get; set; }
            public string PageIcon { get; set; }
            public string PageSlug { get; set; }
            public string Title { get; set; }
            public string PageType { get; set; }

            public bool IsHidden { get; set; }
            public bool IsNewTab { get; set; }
            public bool IsAccessDenied { get; set; }

            public int Sequence { get; set; }

            public DateTimeOffset? AuthorDate { get; set; }
            public DateTimeOffset? LastChangeTime { get; set; }
            public string LastChangeType { get; set; }
            public string LastChangeUser { get; set; }

            public virtual QSite Site { get; set; }
            public virtual QPage Parent { get; set; }

            public ContentContainer Content { get; set; }
            public List<QPageNode> Children { get; set; }
        }

        #endregion

        public static string GetIconName(string type)
        {
            if (type == "Site")
                return "cloud";
            else if (type == "Folder")
                return "folder";
            else if (type == "Template")
                return "crop-alt";
            else if (type == "Page")
                return "file";
            else if (type == "Block")
                return "layer-group";
            else
                return "cube";
        }

        public static string GetIconCssClass(string type) =>
            "far fa-" + GetIconName(type);

        public static string GetEditUrl(Guid id, string type)
        {
            if (type == "Site")
                return $"/ui/admin/sites/outline?id={id}";
            else
                return $"/ui/admin/sites/pages/outline?id={id}";
        }

        public static string GetSearchUrl(string type, bool appendType)
        {
            if (type == "Site")
                return "/ui/admin/sites/sites/search";

            var url = "/ui/admin/sites/pages/search";

            if (appendType)
                url += "?type=" + HttpUtility.UrlEncode(type);

            return url;
        }

        public static string SanitizeSiteName(string value) =>
            StringHelper.Sanitize(value, '-', extraChars: new[] { '.' });

        public static string SanitizeResourceName(string value) =>
            StringHelper.Sanitize(value, '-');

        public static QSite CreateSite(string name, string title)
        {
            var organization = CurrentSessionState.Identity.Organization;
            var user = CurrentSessionState.Identity.User;

            return new QSite
            {
                SiteIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = organization.Key,
                SiteTitle = title,
                SiteDomain = SanitizeSiteName(name)
            };
        }

        public static QSite CopySite(QSite source, string name, string title)
        {
            var user = CurrentSessionState.Identity.User;
            var clone = new QSite
            {
                SiteIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = source.OrganizationIdentifier,
                SiteTitle = title,
                SiteDomain = name
            };

            return clone;
        }

        public static QPage CreatePage(string type, string title) =>
            CreatePage(type, title, title);

        public static QPage CreatePage(string type, string name, string title)
        {
            var page = CreatePage(type);

            page.PageSlug = SanitizeSiteName(name);
            page.Title = title;

            return page;
        }

        public static QPage CreatePage(string type)
        {
            var organization = CurrentSessionState.Identity.Organization;
            var user = CurrentSessionState.Identity.User;

            return new QPage
            {
                PageIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = organization.Key,
                PageType = type,
                AuthorDate = DateTimeOffset.Now,
                AuthorName = user.FullName,
                ContentLabels = "PageBlocks, Body, Title, Summary, ImageURL",
                IsHidden = false
            };
        }

        public static QPage CopyPage(QPage source)
        {
            var organization = CurrentSessionState.Identity.Organization;
            var user = CurrentSessionState.Identity.User;

            var clone = new QPage
            {
                AuthorDate = DateTimeOffset.Now,
                AuthorName = user.FullName,
                ContentLabels = source.ContentLabels,
                PageIcon = source.PageIcon,
                IsHidden = true,
                PageSlug = source.PageSlug,
                NavigateUrl = source.NavigateUrl,
                ParentPageIdentifier = source.ParentPageIdentifier,
                OrganizationIdentifier = organization.OrganizationIdentifier,
                Title = source.Title,
                ContentControl = source.ContentControl,
                PageIdentifier = UniqueIdentifier.Create(),
                PageType = source.PageType,
                SiteIdentifier = source.SiteIdentifier,

            };

            return clone;
        }

        public static QPage[] CopyPages(QPage[] webPages, Guid webSiteIdentifier)
        {
            var user = CurrentSessionState.Identity.User;
            if (webPages.IsNotEmpty())
            {
                var results = new QPage[webPages.Length];
                int index = 0;
                foreach (QPage source in webPages)
                {
                    var clone = new QPage
                    {
                        AuthorDate = DateTimeOffset.Now,
                        AuthorName = user.FullName,
                        ContentLabels = source.ContentLabels,
                        PageIcon = source.PageIcon,
                        IsHidden = true,
                        PageSlug = source.PageSlug,
                        NavigateUrl = source.NavigateUrl,
                        ParentPageIdentifier = source.ParentPageIdentifier,
                        OrganizationIdentifier = source.OrganizationIdentifier,
                        Title = source.Title,
                        ContentControl = source.ContentControl,
                        PageIdentifier = UniqueIdentifier.Create(),
                        PageType = source.PageType,
                        SiteIdentifier = webSiteIdentifier
                    };
                    results[index] = clone;
                    index++;
                }
                return results;
            }
            return null;
        }

        public static TContent[] CopyContent(TContent[] contents, Guid webSiteIdentifier)
        {
            throw new NotImplementedException();
        }

        public static QSiteTree GetQSiteTree(QSite site)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(site.SiteIdentifier);

            var tree = new QSiteTree
            {
                Name = site.SiteDomain,
                Title = site.SiteTitle,
                Content = content,
                SiteIdentifier = site.SiteIdentifier,
                OrganizationIdentifier = site.OrganizationIdentifier,
                Pages = new List<QPageNode>()
            };

            var pages = ServiceLocator.PageSearch.Bind(
                x => x,
                x => x.SiteIdentifier == site.SiteIdentifier && x.ParentPageIdentifier == null,
                "Sequence"
            );

            foreach (var page in pages)
            {
                var serializedPage = GetPageNote(page);
                tree.Pages.Add(serializedPage);
            }

            return tree;
        }

        private static QPageNode GetPageNote(QPage page)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(page.PageIdentifier);
            var groups = TGroupPermissionSearch.Select(x => x.ObjectIdentifier == page.PageIdentifier, null, x => x.Group);

            var node = new QPageNode
            {
                PageType = page.PageType,
                Title = page.Title,
                IsHidden = page.IsHidden,
                PageSlug = page.PageSlug,
                NavigateUrl = page.NavigateUrl,
                IsNewTab = page.IsNewTab,
                PageIcon = page.PageIcon,
                ContentLabels = page.ContentLabels,
                ContentControl = page.ContentControl,
                Hook = page.Hook,
                Content = content,
                PageIdentifier = page.PageIdentifier,
                ParentPageIdentifier = page.ParentPageIdentifier,
                SiteIdentifier = page.SiteIdentifier,
                IsAccessDenied = page.IsAccessDenied,
                Sequence = page.Sequence,
                OrganizationIdentifier = page.OrganizationIdentifier,
            };

            var children = ServiceLocator.PageSearch.Select(page.PageIdentifier, x => x.Children)
                .Children
                .OrderBy(x => x.Sequence)
                .ToList();

            if (children.Count > 0)
            {
                node.Children = new List<QPageNode>();

                foreach (var child in children)
                {
                    var serializedChild = GetPageNote(child);
                    node.Children.Add(serializedChild);
                }
            }

            return node;
        }


        #region Custom Help Pages

        public static QPage GetCustomHelpPage(string actionUrl, Domain.Foundations.User user, OrganizationState organization, bool isHidden = false)
        {
            var domain = ServiceLocator.AppSettings.Security.Domain;
            var siteName = $"{organization.Code}.{domain}";
            var site = ServiceLocator.SiteSearch.BindFirst(x => x, x => x.SiteDomain == siteName && x.OrganizationIdentifier == organization.Identifier);
            if (site == null)
                return null;

            var folder = ServiceLocator.PageSearch.Select(x => x.PageSlug == "in-help" && x.OrganizationIdentifier == organization.Identifier).FirstOrDefault();
            if (folder == null)
                return null;

            if (actionUrl.StartsWith("/"))
                actionUrl = actionUrl.Substring(1, actionUrl.Length - 1);

            var slug = actionUrl.Replace("/", "-");
            var page = ServiceLocator.PageSearch.Select(x => x.PageSlug == slug && x.ParentPageIdentifier == folder.PageIdentifier && x.OrganizationIdentifier == organization.Identifier).FirstOrDefault();
            if (page == null)
                return null;

            return page;
        }

        public static Guid CreateCustomHelpPage(string actionUrl, Domain.Foundations.User user, OrganizationState organization, bool isHidden = false)
        {
            var domain = ServiceLocator.AppSettings.Security.Domain;
            var siteName = $"{organization.Code}.{domain}";
            var site = ServiceLocator.SiteSearch.BindFirst(x => x, x => x.SiteDomain == siteName && x.OrganizationIdentifier == organization.Identifier);
            if (site == null)
                site = CreatePortal(siteName, user, organization);

            var folder = ServiceLocator.PageSearch.Select(x => x.PageSlug == "in-help" && x.OrganizationIdentifier == organization.Identifier).FirstOrDefault();
            if (folder == null)
                folder = CreateFolder("in-help", site.SiteIdentifier, user, organization, isHidden);

            if (actionUrl.StartsWith("/"))
                actionUrl = actionUrl.Substring(1, actionUrl.Length - 1);

            var slug = actionUrl.Replace("/", "-");
            var page = ServiceLocator.PageSearch.Select(x => x.PageSlug == slug && x.ParentPageIdentifier == folder.PageIdentifier && x.OrganizationIdentifier == organization.Identifier).FirstOrDefault();
            if (page == null)
                page = CreatePage(slug, folder.PageIdentifier, site.SiteIdentifier, user, organization);

            return page.PageIdentifier;
        }

        public static QSite CreatePortal(string name, Domain.Foundations.User user, OrganizationState organization)
        {
            var site = new QSite
            {
                SiteDomain = name,
                OrganizationIdentifier = organization.Identifier,
                SiteTitle = $"Custom Inline Help",
                SiteIdentifier = UniqueIdentifier.Create()
            };

            var commands = new SiteCommandGenerator().GetCommands(site);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            return site;
        }

        private static QPage CreateFolder(string slug, Guid site, Domain.Foundations.User user, OrganizationState organization, bool isHidden = false)
        {
            var page = new QPage
            {
                AuthorDate = DateTimeOffset.Now,
                AuthorName = user.FullName,
                ContentLabels = "Title, Summary, Body",
                OrganizationIdentifier = organization.Identifier,
                Title = $"Custom Inline Help",
                PageSlug = slug,
                PageIdentifier = UniqueIdentifier.Create(),
                PageType = "Folder",
                IsHidden = isHidden,
                SiteIdentifier = site
            };

            var commands = new PageCommandGenerator().GetCommands(page);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            return page;
        }

        private static QPage CreatePage(string slug, Guid folder, Guid site, Domain.Foundations.User user, OrganizationState organization)
        {
            var page = new QPage
            {
                AuthorDate = DateTimeOffset.Now,
                AuthorName = user.FullName,
                ContentLabels = "Body",
                ParentPageIdentifier = folder,
                OrganizationIdentifier = organization.Identifier,
                Title = StringHelper.TruncateString($"Custom Inline Help for {slug}", 128),
                PageSlug = slug,
                PageIdentifier = UniqueIdentifier.Create(),
                PageType = "Page",
                SiteIdentifier = site
            };

            var commands = new PageCommandGenerator().GetCommands(page);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            return page;
        }

        #endregion
    }
}