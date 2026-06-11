using System;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application;
using InSite.Application.Organizations.Read;
using InSite.Application.Sites.Read;
using InSite.Domain.Sites.Pages;

using Shift.Common;

namespace InSite.Persistence
{
    public class PageStore : IPageStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        private readonly Action<Exception> _error;
        private readonly IIdentityService _identityService;
        private readonly IOrganizationSearch _organizationSearch;

        public PageStore(Action<Exception> error, IIdentityService identityService, IOrganizationSearch organizationSearch)
        {
            _error = error;
            _identityService = identityService;
            _organizationSearch = organizationSearch;
        }

        #region Inserts

        public void InsertPage(PageCreated e)
        {
            using (var db = CreateContext())
            {
                var sequence = e.Sequence;

                if (sequence == 0)
                {
                    if (e.ParentPage.HasValue)
                    {
                        var maxSequence = db.QPages
                            .Where(x => x.ParentPageIdentifier == e.ParentPage.Value)
                            .Max(x => (int?)x.Sequence);

                        sequence = (maxSequence ?? 0) + 1;
                    }
                    else if (e.Site.HasValue)
                    {
                        var maxSequence = db.QPages
                            .Where(x => !x.ParentPageIdentifier.HasValue && x.SiteIdentifier == e.Site.Value)
                            .Max(x => (int?)x.Sequence);

                        sequence = (maxSequence ?? 0) + 1;
                    }
                }

                var page = new QPage
                {
                    PageIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.OriginOrganization,
                    SiteIdentifier = e.Site,
                    ParentPageIdentifier = e.ParentPage,

                    IsNewTab = e.IsNewTab,
                    IsHidden = e.IsHidden,

                    Sequence = sequence,

                    Title = e.Title,
                    PageType = e.Type
                };

                SetLastChange(page, e);

                db.QPages.Add(page);
                db.SaveChanges();
            }

            ResetCache();
        }

        #endregion

        #region Updates

        public void UpdatePage(AuthorNameChanged e)
        {
            Update(e, page =>
            {
                page.AuthorName = e.AuthorName;
            });
        }

        public void UpdatePage(ContentControlChanged e)
        {
            Update(e, page =>
            {
                page.ContentControl = e.ContentControl;
            });
        }

        public void UpdatePage(ContentLabelsChanged e)
        {
            Update(e, page =>
            {
                page.ContentLabels = e.ContentLabels;
            });
        }

        public void UpdatePage(HookChanged e)
        {
            Update(e, page =>
            {
                page.Hook = e.Hook;
            });
        }

        public void UpdatePage(IconChanged e)
        {
            Update(e, page =>
            {
                page.PageIcon = e.Icon;
            });
        }

        public void UpdatePage(NavigationUrlChanged e)
        {
            Update(e, page =>
            {
                page.NavigateUrl = e.NavigateUrl;
            });
        }

        public void UpdatePage(NewTabValueChanged e)
        {
            Update(e, page =>
            {
                page.IsNewTab = e.IsNewTab;
            });
        }

        public void UpdatePage(SequenceChanged e)
        {
            Update(e, page =>
            {
                page.Sequence = e.Sequence;
            });
        }

        public void UpdatePage(SlugChanged e)
        {
            var slug = e.Slug;
            if (slug != null && slug.Length > 100)
                slug = slug.Substring(0, 100);

            Update(e, page =>
            {
                page.PageSlug = slug;
            });
        }

        public void UpdatePage(TitleChanged e)
        {
            Update(e, page =>
            {
                page.Title = e.Title;
            });
        }

        public void UpdatePage(TypeChanged e)
        {
            Update(e, page =>
            {
                page.PageType = e.Type;
            });
        }

        public void UpdatePage(VisibilityChanged e)
        {
            Update(e, page =>
            {
                page.IsHidden = e.IsHidden;
            });
        }

        public void UpdatePage(AuthorDateChanged e)
        {
            Update(e, page =>
            {
                page.AuthorDate = e.AuthorDate;
            });
        }

        public void UpdatePage(ParentChanged e)
        {
            Update(e, page =>
            {
                page.ParentPageIdentifier = e.Parent;
            });
        }

        public void UpdatePage(SiteChanged e)
        {
            Update(e, page =>
            {
                page.SiteIdentifier = e.Site;
            });
        }

        public void UpdatePage(SurveyChanged e)
        {
            Update(e, page =>
            {
                page.ObjectType = "Survey";
                page.ObjectIdentifier = e.Survey;
            });
        }

        public void UpdatePage(CourseChanged e)
        {
            Update(e, page =>
            {
                page.ObjectType = "Course";
                page.ObjectIdentifier = e.Course;
            });
        }

        public void UpdatePage(PageAssessmentChanged e)
        {
            Update(e, page =>
            {
                page.ObjectType = "Assessment";
                page.ObjectIdentifier = e.Assessment;
            });
        }

        public void UpdatePage(PageObjectModified e)
        {
            Update(e, page =>
            {
                page.ObjectIdentifier = e.Object;
                page.ObjectType = e.Object.HasValue ? e.Type : null;
            });
        }

        public void UpdatePage(ProgramChanged e)
        {
            Update(e, page =>
            {
                page.ObjectType = "Program";
                page.ObjectIdentifier = e.Program;
            });
        }

        public void UpdatePage(PageContentChanged e)
        {
            Update(e, site =>
            {

            });
        }

        private void Update(IChange e, Action<QPage> change)
        {
            using (var db = CreateContext())
            {
                // 2025-12-11: Aleksey - we cannot use AsNoTracking() here because Page is supposed to be changed in "change(page)"
                var page = db.QPages
                    .FirstOrDefault(x => x.PageIdentifier == e.AggregateIdentifier);

                if (page == null)
                    return;

                SetLastChange(page, e);

                change(page);

                db.SaveChanges();

                if (page.Site != null)
                {
                    var isPublicSite = !page.Site.SiteIsPortal;

                    if (isPublicSite)
                        ResetCache();
                }
            }
        }

        /// <remarks>
        /// Public sites are hosted by the Polaris.UI web application, and content is edited inside 
        /// that application. Only website content for www.bcpvpa.org is edited here inside the 
        /// InSite.UI web application. When a BCPVPA page is edited, the BCPVPA site needs to be 
        /// notified to reset its cache. The number of cache resets is limited to a maximum of one 
        /// per minute. If the BCPVPA site is inaccessible for any reason then the error is reported
        /// to Sentry and execution continues normally.
        /// </remarks>
        private void ResetCache()
        {
            try
            {
                var orgId = _identityService?.GetCurrentOrganization();
                var cacheResetUrl = orgId.HasValue 
                    ? _organizationSearch.GetModel(orgId.Value)?.Toolkits.Sites.PublicSiteCacheResetUrl 
                    : null;

                if (cacheResetUrl.IsEmpty())
                    return;

                if (LastReset != null && (DateTimeOffset.Now - LastReset.Value).TotalMinutes < 1)
                    return;

                Shift.Common.TaskRunner.RunSync(StaticHttpClient.Client.GetAsync, cacheResetUrl);
            }
            catch (Exception ex)
            {
                _error?.Invoke(ex);
            }
            finally
            {
                LastReset = DateTimeOffset.Now;
            }
        }
        private static DateTimeOffset? LastReset { get; set; }

        #endregion

        #region Deletes

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                var sql = @"DELETE [sites].[QPage]";
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public void DeleteOne(Guid aggregate)
        {
            using (var db = CreateContext())
            {
                var sql = @"DELETE [sites].[QPage] WHERE [PageIdentifier] = @Aggregate";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@Aggregate", aggregate));
            }
        }

        public void DeletePage(PageDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QPages.FirstOrDefault(x => x.PageIdentifier == e.AggregateIdentifier);
                if (entity != null)
                    db.QPages.Remove(entity);

                db.SaveChanges();
            }

            ResetCache();
        }

        #endregion

        private void SetLastChange(QPage page, IChange e)
        {
            page.LastChangeTime = e.ChangeTime;
            page.LastChangeType = e.GetType().Name;
            page.LastChangeUser = UserSearch.GetFullName(e.OriginUser);
        }
    }
}