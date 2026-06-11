using System;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Sites.Read;
using InSite.Domain.Sites.Sites;

namespace InSite.Persistence
{
    public class SiteStore : ISiteStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        public void InsertSite(SiteCreated e)
        {
            using (var db = CreateContext())
            {
                var site = new QSite
                {
                    SiteIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.OriginOrganization,

                    SiteDomain = e.Domain,
                    SiteTitle = e.Title
                };

                SetLastChange(site, e);

                db.QSites.Add(site);
                db.SaveChanges();
            }
        }

        public void UpdateSite(SiteContentChanged e)
        {
            Update(e, site =>
            {

            });
        }

        public void UpdateSite(SiteTitleChanged e)
        {
            Update(e, site =>
            {
                site.SiteTitle = e.Title;
            });
        }

        public void UpdateSite(SiteDomainChanged e)
        {
            Update(e, site =>
            {
                site.SiteDomain = e.Domain;
            });
        }

        public void UpdateSite(SiteConfigurationChanged e)
        {
            Update(e, site =>
            {
                
            });
        }

        public void UpdateSite(SiteTypeChanged e)
        {
            Update(e, site =>
            {
                
            });
        }

        private void Update(IChange e, Action<QSite> change)
        {
            using (var db = CreateContext())
            {
                var site = db.QSites
                    .FirstOrDefault(x => x.SiteIdentifier == e.AggregateIdentifier);

                if (site == null)
                    return;

                SetLastChange(site, e);
                change(site);
                db.SaveChanges();
            }
        }

        private void SetLastChange(QSite site, IChange e)
        {
            site.LastChangeTime = e.ChangeTime;
            site.LastChangeType = e.GetType().Name;
            site.LastChangeUser = UserSearch.GetFullName(e.OriginUser);
        }

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                var sql = @"DELETE [sites].[QSite]";
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public void DeleteOne(Guid aggregate)
        {
            using (var db = CreateContext())
            {
                var sql = @"DELETE [sites].[QSite] WHERE [SiteIdentifier] = @Aggregate";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@Aggregate", aggregate));
            }
        }

        public void DeleteSite(SiteDeleted change)
        {
            using (var db = CreateContext())
            {
                var site = db.QSites.FirstOrDefault(x => x.SiteIdentifier == change.AggregateIdentifier);
                if (site != null)
                    db.QSites.Remove(site);

                db.SaveChanges();
            }
        }
    }
}