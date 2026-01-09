using System;
using System.Linq;

namespace InSite.Persistence
{
    public class VWebPageHierarchySearch
    {
        public static int CountPages(Guid organization, string path)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .Count(x => x.OrganizationIdentifier == organization && x.PathUrl.StartsWith(path));
            }
        }

        public static VWebPageHierarchy GetPage(Guid organization, string path)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .FirstOrDefault(x => x.OrganizationIdentifier == organization && x.PathUrl == path);
            }
        }

        public static VWebPageHierarchy GetPage(Guid page)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .FirstOrDefault(x => x.WebPageIdentifier == page);
            }
        }

        public static VWebPageHierarchy[] GetPages(Guid organization, string domain)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .Where(x => x.OrganizationIdentifier == organization && x.WebSiteDomain == domain)
                    .ToArray();
            }
        }

        public static VWebPageHierarchy[] GetPages(Guid siteId)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .Where(x => x.WebSiteIdentifier == siteId)
                    .OrderBy(x => x.PathSequence).ThenBy(x => x.PathUrl)
                    .ToArray();
            }
        }

        public static VWebPageHierarchy[] GetChildren(Guid page)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .Where(x => x.ParentWebPageIdentifier == page)
                    .OrderBy(x => x.PathSequence).ThenBy(x => x.PathUrl)
                    .ToArray();
            }
        }

        public static VWebPageHierarchy GetWebPage(string domain, string path)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .Where(x => x.WebSiteDomain == domain && x.PathUrl == path)
                    .FirstOrDefault();
            }
        }

        internal static bool HelpTopicExists(string helpUrl)
        {
            if (string.IsNullOrEmpty(helpUrl))
                return false;

            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                return db.VWebPageHierarchies.AsNoTracking()
                    .Any(x => helpUrl.EndsWith("help/" + x.PathUrl));
            }
        }
    }
}
